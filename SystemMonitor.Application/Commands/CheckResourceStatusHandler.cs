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
        var currentStatus = await resource.IsOkayAsync(cancellationToken);

        if (currentStatus)
        {
            return Result.Success();
        }

        var lastStatus = await lastStatusRepository.GetAsync(resource.Name, cancellationToken);

        if (lastStatus is null || !currentStatus)
        {
            // No last status yet recorded or the resource is still in a bad state.

            return Result.Success();
        }

        var message = await resource.GetStatusInformationAsync(cancellationToken);

        eventRepository.Add(new Event
        {
            Resource = resource.Name,
            LoggedAt = DateTime.UtcNow,
            Status = message
        });

        const int expectedChanges = 1;

        int actualChanges = await context.SaveChangesAsync();

        if (expectedChanges != actualChanges)
        {
            Logger.LogError("Command '{Command}' for resource '{Resource}' wrote an unexpected number of changes to the database: expected '{Expected}', actual '{Actual}'.", CommandName, resource.Name, expectedChanges, actualChanges);
        }

        Logger.LogInformation("Command '{Command}' found an issue with resource '{Resource}': {Message}", CommandName, resource.Name, message);

        return Result.DomainError("Issue found with resource.");
    }

    public Task<Result> ExecuteAsync(CancellationToken cancellationToken) => base.ExecuteAsync(new CheckResourceStatusCommand(), cancellationToken);
}

public interface ICheckResourceStatus
{
    Task<Result> ExecuteAsync(CancellationToken cancellationToken = default);

    int FrequencyInSeconds { get; }

    string ResourceName { get; }
}

public sealed class CheckResourceStatusCommand : ICommand;
