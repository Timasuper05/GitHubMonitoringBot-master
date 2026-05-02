using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GitHub_Monitoring_Bot;

public class GitHubMonitorWorker(
    AppSettings settings,
    IServiceScopeFactory scopeFactory,
    ILogger<GitHubMonitorWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Start(stoppingToken);
            await Task.Delay(settings.Interval, stoppingToken);
        }
    }

    private async Task Start(CancellationToken stoppingToken)
    {
        try
        {
            logger.LogInformation("GitHub check started");
            using var scope = scopeFactory.CreateScope();
            var monitoringService = scope.ServiceProvider.GetRequiredService<PullRequestMonitoringService>();
            await monitoringService.CheckTrackedRepositoriesPullRequestsAsync(stoppingToken);
            logger.LogInformation("GitHub check completed");
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GitHub check failed");
        }
    }
}
