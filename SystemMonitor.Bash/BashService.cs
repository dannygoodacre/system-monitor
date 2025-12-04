using System.Diagnostics;
using SystemMonitor.Application.Abstractions.Models;
using SystemMonitor.Application.Abstractions.Services;

namespace SystemMonitor.Bash;

internal sealed class BashService : ICommandService
{
    public async Task<ExecutionResult> ExecuteAsync(string command, CancellationToken cancellationToken)
    {
        using var process = new Process();

        process.StartInfo = new ProcessStartInfo
        {
            FileName = "/bin/bash",
            Arguments = $"-c \"{command}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        process.Start();

        string output = await process.StandardOutput.ReadToEndAsync(cancellationToken);

        string error = await process.StandardError.ReadToEndAsync(cancellationToken);

        await process.WaitForExitAsync(cancellationToken);

        return new ExecutionResult
        {
            ExitCode = process.ExitCode,
            Output = output,
            Error = error,
            Success = process.ExitCode == 0
        };
    }
}
