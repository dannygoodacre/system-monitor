namespace SystemMonitor.Application.Abstractions.Models;

public sealed record ExecutionResult
{
    public int ExitCode { get; init; }

    public string Output { get; init; } = string.Empty;

    public string Error { get; init; } = string.Empty;

    public bool Success { get; init; }
}
