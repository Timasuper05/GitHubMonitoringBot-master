using GitHub_Monitoring_Bot.Mapping;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Octokit;
using Telegram.Bot;

namespace GitHub_Monitoring_Bot.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationSettings(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AppSettings>(configuration);
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<AppSettings>>().Value);

        return services;
    }

    public static IServiceCollection AddMapping(this IServiceCollection services)
    {
        var mapsterConfig = TypeAdapterConfig.GlobalSettings;
        mapsterConfig.Scan(typeof(MappingConfig).Assembly);

        services.AddSingleton(mapsterConfig);
        services.AddScoped<IMapper, ServiceMapper>();
        services.AddScoped<PullRequestRecordMapper>();

        return services;
    }

    public static IServiceCollection AddGitHubClient(this IServiceCollection services)
    {
        services.AddSingleton<GitHubClient>(sp =>
        {
            var settings = sp.GetRequiredService<AppSettings>();
            var client = new GitHubClient(new ProductHeaderValue("GitHub-Monitoring"));
            client.Credentials = new Credentials(settings.GitPat);

            return client;
        });

        return services;
    }

    public static IServiceCollection AddNotifications(this IServiceCollection services)
    {
        services.AddSingleton<TelegramBotClient>(sp =>
        {
            var settings = sp.GetRequiredService<AppSettings>();
            return new TelegramBotClient(settings.BotToken);
        });
        services.AddSingleton<INotification, TelegramNotificationService>();
        services.AddScoped<PullRequestNotificationFormatter>();
        services.AddScoped<PullRequestNotificationService>();

        return services;
    }

    public static IServiceCollection AddMonitoringServices(this IServiceCollection services)
    {
        services.AddScoped<GitHubPullRequestService>();
        services.AddScoped<PullRequestStatusResolver>();
        services.AddScoped<PullRequestMonitoringService>();
        services.AddHostedService<GitHubMonitorWorker>();

        return services;
    }

    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddDbContext<DataContext>();
        services.AddScoped<PullRequestRecordRepository>();
        services.AddScoped<DatabaseInitializer>();

        return services;
    }
}
