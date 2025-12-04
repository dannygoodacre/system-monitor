using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SystemMonitor.Core.Extensions;

namespace SystemMonitor.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        var handlerTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(x =>
                x is { IsAbstract: false, IsClass: true } && x.InheritsFromCommandHandler());

        foreach (var handlerType in handlerTypes)
        {
            var interfaces = handlerType.GetInterfaces();

            foreach (var serviceType in interfaces)
            {
                services.AddScoped(serviceType, handlerType);
            }
        }

        return services;
    }

    public static IServiceCollection AddQueryHandlers(this IServiceCollection services)
    {
        var handlerTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(x =>
                x is { IsAbstract: false, IsClass: true } && x.InheritsFromQueryHandler());

        foreach (var handlerType in handlerTypes)
        {
            var interfaces = handlerType.GetInterfaces();

            foreach (var serviceType in interfaces)
            {
                services.AddScoped(serviceType, handlerType);
            }
        }

        return services;
    }
}
