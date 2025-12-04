using SystemMonitor.Application.Abstractions.Data;
using SystemMonitor.Application.Abstractions.Data.Repositories;
using SystemMonitor.Application.Commands;
using SystemMonitor.Application.Resources;

namespace SystemMonitor;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHostedServices(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceProvider = services.BuildServiceProvider();

        var resources = serviceProvider.GetServices<IResource>().ToList();

        foreach (var resource in resources)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<CheckResourceStatusHandler>>();

            var eventRepository = serviceProvider.GetRequiredService<IEventRepository>();

            var context = serviceProvider.GetRequiredService<IApplicationContext>();

            var resourceStatusChecker = new CheckResourceStatusHandler(logger, resource, eventRepository, context);

            services.AddScoped<ICheckResourceStatus>(_ => resourceStatusChecker);

            var sendWarningEmailHandler = serviceProvider.GetRequiredService<ISendWarningEmail>();

            services.AddHostedService(_ => new ResourceMonitorService(logger, resourceStatusChecker, sendWarningEmailHandler));
        }

        return services;
    }
}
