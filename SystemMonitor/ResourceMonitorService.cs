using SystemMonitor.Application.Commands;
using SystemMonitor.Core.Common;

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

            if (statusResult.IsSuccess)
            {
                await Task.Delay(TimeSpan.FromSeconds(checkResourceStatus.FrequencyInSeconds), cancellationToken);

                continue;
            }

            if (statusResult.Status == Status.DomainError)
            {
                var sendEmailResult = await sendWarningEmail.ExecuteAsync(checkResourceStatus.ResourceName, cancellationToken);

                if (!sendEmailResult.IsSuccess)
                {
                    logger.LogCritical("Could not send a warning email for the resource '{Resource}'.", checkResourceStatus.ResourceName);
                }
            }
            else
            {
                logger.LogCritical("Could not verify the state of the resource '{Resource}'.", checkResourceStatus.ResourceName);
            }

            await Task.Delay(TimeSpan.FromSeconds(checkResourceStatus.FrequencyInSeconds), cancellationToken);
        }
    }
}
