using Microsoft.Extensions.Logging;
using SystemMonitor.Application.Abstractions.Data;
using SystemMonitor.Application.Abstractions.Data.Repositories;
using SystemMonitor.Application.Resources;
using SystemMonitor.Core.CommandQuery;
using SystemMonitor.Core.Common;
using SystemMonitor.Domain.Models;

namespace SystemMonitor.Application.Commands;

public class CheckResourceStatusHandler(ILogger<CheckResourceStatusHandler> logger,
                                               IResource resource,
                                               IEventRepository repository,
                                               IApplicationContext context) : CommandHandler<CheckResourceStatusCommand>(logger), ICheckResourceStatus
{
    public int FrequencyInSeconds => resource.CheckFrequencyInSeconds;

    public string ResourceName => resource.Name;

    protected override string CommandName => $"Check Resource Status";

    protected override async Task<Result> InternalExecuteAsync(CheckResourceStatusCommand command, CancellationToken cancellationToken)
    {
        if (await resource.IsOkayAsync(cancellationToken))
        {
            return Result.Success();
        }

        var message = await resource.GetStatusInformationAsync(cancellationToken);

        repository.Add(new Event
        {
            Resource = resource.Name,
            LoggedAt = DateTime.UtcNow,
            Message = message
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
