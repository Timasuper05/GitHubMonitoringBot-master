using Microsoft.Extensions.Logging;

namespace GitHub_Monitoring_Bot;

public class PullRequestNotificationService(
    IEnumerable<INotification> notifications,
    PullRequestNotificationFormatter formatter,
    ILogger<PullRequestNotificationService> logger)
{
    public async Task SendPullRequestNotificationAsync(
        PullRequestRecord pullRequestRecord,
        CancellationToken cancellationToken)
    {
        var message = formatter.BuildMessage(pullRequestRecord);
        foreach (var notification in notifications)
        {
            await notification.SendMessageAsync(message, cancellationToken);
            logger.LogInformation(
                "Pull request notification for {Owner}/{RepositoryName} #{PullRequestNumber} was sent by {NotificationType}",
                pullRequestRecord.RepoOwner,
                pullRequestRecord.RepoName,
                pullRequestRecord.PrNumber,
                notification.GetType().Name);
        }
    }
}
