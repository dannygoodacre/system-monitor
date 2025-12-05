using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SystemMonitor.Application.Abstractions.Data;
using SystemMonitor.Application.Abstractions.Data.Repositories;
using SystemMonitor.Data.Repositories;

// ReSharper disable once CheckNamespace
namespace SystemMonitor.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationContext>(options =>
        {
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));

            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        services.AddScoped<IEventRepository, EventRepository>();

        services.AddScoped<IApplicationContext>(x => x.GetRequiredService<ApplicationContext>());

        return services;
    }
}
