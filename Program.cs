using GitHub_Monitoring_Bot;
using GitHub_Monitoring_Bot.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHost host = Host.CreateDefaultBuilder(args).ConfigureServices((context, service) =>
{
    service
        .AddApplicationSettings(context.Configuration)
        .AddMapping()
        .AddGitHubClient()
        .AddNotifications()
        .AddMonitoringServices()
        .AddPersistence();
}).Build();

using (var scope = host.Services.CreateScope())
{
    var databaseInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await databaseInitializer.ApplyMigrationsAsync();
}

await host.RunAsync();
