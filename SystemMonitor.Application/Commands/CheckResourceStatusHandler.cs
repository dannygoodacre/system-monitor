using Microsoft.Extensions.Logging;
using SystemMonitor.Application.Abstractions.Data;
using SystemMonitor.Application.Abstractions.Data.Repositories;
using SystemMonitor.Application.Resources;
using SystemMonitor.Core.CommandQuery;
using SystemMonitor.Core.Common;
using SystemMonitor.Domain.Entities;

namespace SystemMonitor.Application.Commands;

internal sealed class CheckResourceStatusHandler(ILogger<CheckResourceStatusHandler> logger,
                                                 IResource resource,
                                                 IEventRepository eventRepository,
                                                 ILastStatusRepository lastStatusRepository,
                                                 IApplicationContext context) : CommandHandler<CheckResourceStatusCommand>(logger), ICheckResourceStatus
{
    public int FrequencyInSeconds => resource.CheckFrequencyInSeconds;

    public string ResourceName => resource.Name;

    protected override string CommandName => $"Check Resource Status";

    protected override async Task<Result> InternalExecuteAsync(CheckResourceStatusCommand command, CancellationToken cancellationToken)
    {
        var isCurrentlyOkay = await resource.IsOkayAsync(cancellationToken);

        var lastStatus = await lastStatusRepository.GetAsync(resource.Name, cancellationToken);

        if (lastStatus is null)
        {
            // create first entry for resource

            lastStatus = new LastStatus
            {
                Resource = resource.Name,
                IsOkay = isCurrentlyOkay,
                Status = isCurrentlyOkay
                    ? ""
                    : await resource.GetStatusInformationAsync(cancellationToken)
            };

            lastStatusRepository.Add(lastStatus);

            await SaveChangesAsync(1);
        }

        if (isCurrentlyOkay)
        {
            // currently OK

            if (lastStatus.IsOkay)
            {
                // previously OK
                return Result.Success();
            }

            // was NOT OK, now is OK
            lastStatus.IsOkay = true;
            lastStatus.Status = "";
            lastStatus.LoggedAt = DateTime.UtcNow;

            await SaveChangesAsync(0);

            return Result.Success();
        }

        // currently NOT OK
        if (!lastStatus.IsOkay)
        {
            // still NOT OK

            // TODO: Send email if passed time limit.

            var timeSinceLastAlert = (DateTime.UtcNow - lastStatus.LastEmailSendAt).TotalSeconds;

            if (timeSinceLastAlert >= resource.EmailFrequencyInSeconds)
            {
                // TODO: Need to refactor how I'm sending emails. Do I return something special from this to indicate 'yes send', or send it directly here?
            }

            return Result.DomainError("Issue found with resource.");
        }

        // previously OK and now NOT OK

        // send message and log event.

        var message = await resource.GetStatusInformationAsync(cancellationToken);

        Logger.LogInformation("Command '{Command}' found an issue with resource '{Resource}': {Message}", CommandName, resource.Name, message);

        eventRepository.Add(new Event
        {
            Resource = resource.Name,
            LoggedAt = DateTime.UtcNow,
            Status = message
        });

        lastStatus.IsOkay = false;
        lastStatus.Status = message;
        lastStatus.LoggedAt = DateTime.UtcNow;
        lastStatus.LastEmailSendAt = DateTime.UtcNow;

        await SaveChangesAsync(1);

        return Result.DomainError("Issue found with resource.");
    }

    public Task<Result> ExecuteAsync(CancellationToken cancellationToken) => base.ExecuteAsync(new CheckResourceStatusCommand(), cancellationToken);

    private async Task SaveChangesAsync(int expectedChanges)
    {
        var actualChanges = await context.SaveChangesAsync();

        if (expectedChanges != actualChanges)
        {
            Logger.LogError("Command '{Command}' for resource '{Resource}' wrote an unexpected number of changes to the database: expected '{Expected}', actual '{Actual}'.", CommandName, resource.Name, expectedChanges, actualChanges);
        }
    }
}

public interface ICheckResourceStatus
{
    Task<Result> ExecuteAsync(CancellationToken cancellationToken = default);

    int FrequencyInSeconds { get; }

    string ResourceName { get; }
}

public sealed class CheckResourceStatusCommand : ICommand;
