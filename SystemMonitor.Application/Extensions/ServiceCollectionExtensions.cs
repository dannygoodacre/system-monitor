using Microsoft.Extensions.DependencyInjection;
using SystemMonitor.Application.Resources;

// ReSharper disable once CheckNamespace
namespace SystemMonitor.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddResources();

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
}
