namespace GitHub_Monitoring_Bot;

public sealed record NotificationMessage(
    string Text,
    string? ButtonText = null,
    string? ButtonUrl = null);
