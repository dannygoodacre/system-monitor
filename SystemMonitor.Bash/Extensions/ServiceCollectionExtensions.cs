using Microsoft.Extensions.DependencyInjection;
using SystemMonitor.Application.Abstractions.Services;

// ReSharper disable once CheckNamespace
namespace SystemMonitor.Bash;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBashService(this IServiceCollection services)
    {
        services.AddScoped<ICommandService, BashService>();

        return services;
    }
}
