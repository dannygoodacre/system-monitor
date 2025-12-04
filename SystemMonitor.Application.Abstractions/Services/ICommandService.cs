using SystemMonitor.Application.Abstractions.Models;

namespace SystemMonitor.Application.Abstractions.Services;

public interface ICommandService
{
    Task<ExecutionResult> ExecuteAsync(string command, CancellationToken cancellationToken = default);
}
