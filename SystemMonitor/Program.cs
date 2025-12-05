using SystemMonitor.Application;
using SystemMonitor.Bash;
using SystemMonitor.Configuration;
using SystemMonitor.Data;
using SystemMonitor.Email;

namespace SystemMonitor;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        if (builder.Environment.IsDevelopment())
        {
            builder.Configuration.AddUserSecrets<Program>();
        }

        builder.Services.AddOptions(builder.Configuration);

        builder.Services.AddData(builder.Configuration);

        builder.Services.AddBashService();
        builder.Services.AddEmailServices(builder.Configuration);

        builder.Services.AddApplication();

        builder.Services.AddHostedServices(builder.Configuration);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            await context.Database.EnsureCreatedAsync();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        await app.RunAsync();
    }
}
