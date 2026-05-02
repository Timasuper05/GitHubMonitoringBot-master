namespace GitHub_Monitoring_Bot;

public class PullRequestNotificationFormatter
{
    public NotificationMessage BuildMessage(PullRequestRecord pullRequestRecord)
    {
        var status = pullRequestRecord.PrStatus switch
        {
            PullRequestStatus.Open => "🚀 New",
            PullRequestStatus.Merged => "✅ Merged",
            PullRequestStatus.Closed => "❌ Closed",
            _ => pullRequestRecord.PrStatus.ToString()
        };

        var eventDate = GetEventDate(pullRequestRecord);
        var eventDateMoscow = ConvertToMoscowTime(eventDate);
        var text =
            $"{status}\n" +
            "\n" +
            $"📌 {pullRequestRecord.PrTitle}\n" +
            "\n" +
            $"👤 Author: {pullRequestRecord.Author}\n" +
            "\n" +
            $"📅 Date: {eventDateMoscow:dd-MM-yyyy HH:mm:ss} время мск";

        return new NotificationMessage(
            text,
            ButtonText: "Открыть PR",
            ButtonUrl: pullRequestRecord.PrUrl);
    }

    private static DateTimeOffset GetEventDate(PullRequestRecord pullRequestRecord)
    {
        return pullRequestRecord.PrStatus switch
        {
            PullRequestStatus.Merged => pullRequestRecord.MergedAt ?? pullRequestRecord.UpdatedAt,
            PullRequestStatus.Closed => pullRequestRecord.ClosedAt ?? pullRequestRecord.UpdatedAt,
            _ => pullRequestRecord.PrCreateAt
        };
    }

    private static DateTimeOffset ConvertToMoscowTime(DateTimeOffset value)
    {
        try
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
            return TimeZoneInfo.ConvertTime(value, timeZone);
        }
        catch (TimeZoneNotFoundException)
        {
            return value.ToUniversalTime().ToOffset(TimeSpan.FromHours(3));
        }
        catch (InvalidTimeZoneException)
        {
            return value.ToUniversalTime().ToOffset(TimeSpan.FromHours(3));
        }
    }
}
