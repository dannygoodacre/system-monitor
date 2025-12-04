using SystemMonitor.Application.Commands;

namespace SystemMonitor;

public class ResourceMonitorService(ILogger logger,
                                    ICheckResourceStatus monitorStrategy,
                                    ISendWarningEmail sendWarningEmail) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var result = await monitorStrategy.ExecuteAsync(cancellationToken);

            if (result.IsNonDomainError)
            {
                logger.LogCritical("Could not verify the state of the resource '{Resource}'.", monitorStrategy.ResourceName);

                await Task.Delay(TimeSpan.FromSeconds(monitorStrategy.FrequencyInSeconds), cancellationToken);

                continue;
            }

            if (result.IsSuccess)
            {
                await Task.Delay(TimeSpan.FromSeconds(monitorStrategy.FrequencyInSeconds), cancellationToken);

                continue;
            }

            var sendEmailResult = await sendWarningEmail.ExecuteAsync(monitorStrategy.ResourceName, cancellationToken);

            if (!sendEmailResult.IsSuccess)
            {
                logger.LogCritical("Could not send a warning email for the resource '{Resource}'.", monitorStrategy.ResourceName);
            }

            await Task.Delay(TimeSpan.FromSeconds(monitorStrategy.FrequencyInSeconds), cancellationToken);
        }
    }
}
