using RaidMonitor.Email;
using SystemMonitor.Application;
using SystemMonitor.Bash;
using SystemMonitor.Core;
using SystemMonitor.Data;
using SystemMonitor.Configuration;

namespace SystemMonitor;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddUserSecrets<Program>();

        builder.Services.AddOptions(builder.Configuration);

        // Add services to the container.
        builder.Services.AddRazorPages();

        builder.Services.AddData(builder.Configuration);

        builder.Services.AddCommandHandlers();
        builder.Services.AddQueryHandlers();

        builder.Services.AddEmailServices(builder.Configuration);
        builder.Services.AddBashService();

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

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.MapStaticAssets();
        app.MapRazorPages()
            .WithStaticAssets();

        app.Run();
    }
}
