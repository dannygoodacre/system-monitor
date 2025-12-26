using SystemMonitor.Application.Commands;

namespace SystemMonitor;

public class ResourceMonitorService(ILogger logger,
                                    ICheckResourceStatus checkResourceStatus,
                                    ISendWarningEmail sendWarningEmail) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var statusResult = await checkResourceStatus.ExecuteAsync(cancellationToken);

            if (!statusResult.IsSuccess)
            {
                logger.LogCritical("Could not verify the state of the resource '{Resource}'.", checkResourceStatus.ResourceName);
            }
            else if (!statusResult.Value.IsOkay && statusResult.Value.ShouldNotify)
            {
                var sendEmailResult = await sendWarningEmail.ExecuteAsync(checkResourceStatus.ResourceName, cancellationToken);

                if (!sendEmailResult.IsSuccess)
                {
                    logger.LogCritical("Could not send a warning email for the resource '{Resource}'.", checkResourceStatus.ResourceName);
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(checkResourceStatus.FrequencyInSeconds), cancellationToken);
        }
    }
}
