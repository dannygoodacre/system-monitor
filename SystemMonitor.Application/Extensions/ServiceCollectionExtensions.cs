using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SystemMonitor.Application.Abstractions.Data;
using SystemMonitor.Application.Abstractions.Data.Repositories;
using SystemMonitor.Application.Commands;
using SystemMonitor.Application.Resources;
using SystemMonitor.Core;

// ReSharper disable once CheckNamespace
namespace SystemMonitor.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddCommandHandlers(typeof(CheckResourceStatusHandler).Assembly);

        services.AddResources();

        services.AddCheckResourceStatusHandlers();

        return services;
    }

    private static IServiceCollection AddResources(this IServiceCollection services)
    {
        var resourceTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IResource).IsAssignableFrom(t) && t is { IsClass: true, IsAbstract: false });

        foreach (var type in resourceTypes)
        {
            services.AddTransient(typeof(IResource), type);
        }

        return services;
    }

    private static IServiceCollection AddCheckResourceStatusHandlers(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();

        var resources = serviceProvider.GetRequiredService<IEnumerable<IResource>>();

        foreach (var resource in resources)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<CheckResourceStatusHandler>>();

            var eventRepository = serviceProvider.GetRequiredService<IEventRepository>();

            var context = serviceProvider.GetRequiredService<IApplicationContext>();

            services.AddScoped<ICheckResourceStatus>(_ => new CheckResourceStatusHandler(logger, resource, eventRepository, context));
        }

        return services;
    }
}
