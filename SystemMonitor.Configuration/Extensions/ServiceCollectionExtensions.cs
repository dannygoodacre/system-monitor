using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SystemMonitor.Configuration.Options;
using SystemMonitor.Configuration.OptionsValidators;

// ReSharper disable once CheckNamespace
namespace SystemMonitor.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<EmailOptions>()
            .Bind(configuration.GetSection("Email"))
            .ValidateOnStart();

        services.AddSingleton<IValidateOptions<EmailOptions>, EmailOptionsValidator>();

        return services;
    }
}
