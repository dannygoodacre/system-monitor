using SystemMonitor.Application.Abstractions.Services;

namespace SystemMonitor.Application.Resources;

internal sealed class ZPoolResource(ICommandService commandService) : IResource
{
    public string Name => "ZPool";

    public int CheckFrequencyInSeconds => 60;

    public async Task<string> GetStatusInformationAsync(CancellationToken cancellationToken = default)
    {
        var result = await commandService.ExecuteAsync("zpool status tank",cancellationToken);

        return result.Output;

        // return await Task.FromResult("Test status information");
    }

    public async Task<bool> IsOkayAsync(CancellationToken cancellationToken = default)
    {
        var result = await commandService.ExecuteAsync("zpool list -H -o health tank",cancellationToken);

        return result.Output.Trim() != "ONLINE";

        // return false;
    }
}
