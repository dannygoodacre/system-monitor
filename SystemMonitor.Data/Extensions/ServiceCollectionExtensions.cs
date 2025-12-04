using Microsoft.AspNetCore.Identity;
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
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services
            .AddDbContext<ApplicationContext>(options => options.UseSqlite(connectionString))
            .AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationContext>();

        // TODO: In development only?
        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddScoped<IEventRepository, EventRepository>();

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IApplicationContext>(provider => provider.GetRequiredService<ApplicationContext>());

        return services;
    }
}
