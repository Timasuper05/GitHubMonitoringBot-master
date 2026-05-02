namespace GitHub_Monitoring_Bot;

public interface INotification
{
    Task SendMessageAsync(NotificationMessage message, CancellationToken cancellationToken);
}
