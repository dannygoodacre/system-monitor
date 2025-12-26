using Microsoft.Extensions.Logging;
using SystemMonitor.Application.Abstractions.Data;
using SystemMonitor.Application.Abstractions.Data.Repositories;
using SystemMonitor.Application.Abstractions.Models;
using SystemMonitor.Application.Extensions;
using SystemMonitor.Application.Resources;
using SystemMonitor.Core.CommandQuery;
using SystemMonitor.Core.Common;
using SystemMonitor.Domain.Entities;

namespace SystemMonitor.Application.Commands;

internal sealed class CheckResourceStatusHandler(ILogger<CheckResourceStatusHandler> logger,
                                                 IResource resource,
                                                 IEventRepository eventRepository,
                                                 ILastStatusRepository lastStatusRepository,
                                                 IApplicationContext context) : CommandHandler<CheckResourceStatusCommand, ResourceStatus>(logger), ICheckResourceStatus
{
    public int FrequencyInSeconds => resource.CheckFrequencyInSeconds;

    public string ResourceName => resource.Name;

    protected override string CommandName => $"Check Resource Status";

    protected override async Task<Result<ResourceStatus>> InternalExecuteAsync(CheckResourceStatusCommand command, CancellationToken cancellationToken)
    {
        var isCurrentlyOkay = await resource.IsOkayAsync(cancellationToken);

        var lastStatus = await lastStatusRepository.GetAsync(resource.Name, cancellationToken);

        if (lastStatus is null)
        {
            lastStatus = new LastStatus
            {
                Resource = resource.Name,
                IsOkay = true,
                Message = ""
            };

            lastStatusRepository.Add(lastStatus);
        }

        if (isCurrentlyOkay)
        {
            if (lastStatus.IsOkay)
            {
                return Result.Success(ResourceStatus.Okay());
            }

            lastStatus.IsOkay = true;
            lastStatus.Message = "";
            lastStatus.LastUpdatedAt = DateTime.UtcNow;

            await SaveChangesAsync(1);

            return Result.Success(ResourceStatus.Okay());
        }

        var message = await resource.GetStatusInformationAsync(cancellationToken);

        if (!lastStatus.IsOkay && lastStatus.SecondsSinceLastNotification() < resource.NotificationFrequencyInSeconds)
        {
            return Result.Success(ResourceStatus.NotOkay(message, false));
        }

        Logger.LogInformation("New issue found with resource '{Resource}': {Status}", resource.Name, message);

        lastStatus.IsOkay = false;
        lastStatus.Message = message;
        lastStatus.LastUpdatedAt = DateTime.UtcNow;
        lastStatus.LastNotifiedAt = DateTime.UtcNow;

        await SaveChangesAsync(1);

        return Result.Success(ResourceStatus.NotOkay(message, true));
    }

    public Task<Result<ResourceStatus>> ExecuteAsync(CancellationToken cancellationToken)
        => base.ExecuteAsync(new CheckResourceStatusCommand(), cancellationToken);

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
    Task<Result<ResourceStatus>> ExecuteAsync(CancellationToken cancellationToken = default);

    int FrequencyInSeconds { get; }

    string ResourceName { get; }
}

public sealed class CheckResourceStatusCommand : ICommand;
