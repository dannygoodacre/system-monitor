using SystemMonitor.Application.Commands;
using SystemMonitor.Application.Resources;

// ReSharper disable once CheckNamespace
namespace SystemMonitor;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHostedServices(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceProvider = services.BuildServiceProvider();

        var checkResourceStatusHandlers = serviceProvider.GetRequiredService<IEnumerable<ICheckResourceStatus>>();

        foreach (var checkResourceStatusHandler in checkResourceStatusHandlers)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<ICheckResourceStatus>>();

            var sendWarningEmailHandler = serviceProvider.GetRequiredService<ISendWarningEmail>();

            services.AddHostedService(_ => new ResourceMonitorService(logger, checkResourceStatusHandler, sendWarningEmailHandler));
        }

        return services;
    }
}
