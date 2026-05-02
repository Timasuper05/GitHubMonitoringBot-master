using Xunit;

namespace GitHub_Monitoring_Bot.Tests;

public class PullRequestNotificationFormatterTests
{
    [Fact]
    public void BuildMessage_creates_text_with_pull_request_button()
    {
        var formatter = new PullRequestNotificationFormatter();
        var record = new PullRequestRecord
        {
            Author = "octocat",
            RepoOwner = "org",
            RepoName = "care-call",
            PrNumber = 42,
            PrTitle = "CCL-214 Добавляет RateLimiter для создания тикетов",
            PrDescription = "Important change",
            PrCreateAt = new DateTimeOffset(2026, 5, 2, 16, 19, 58, TimeSpan.Zero),
            UpdatedAt = new DateTimeOffset(2026, 5, 1, 11, 0, 0, TimeSpan.Zero),
            PrStatus = PullRequestStatus.Open,
            RepoUrl = "https://github.com/org/care-call",
            PrUrl = "https://github.com/org/care-call/pull/42"
        };

        var message = formatter.BuildMessage(record);

        Assert.Equal(
            "🚀 New\n" +
            "\n" +
            "📌 CCL-214 Добавляет RateLimiter для создания тикетов\n" +
            "\n" +
            "👤 Author: octocat\n" +
            "\n" +
            "📅 Date: 02-05-2026 19:19:58 время мск",
            message.Text);
        Assert.Equal("Открыть PR", message.ButtonText);
        Assert.Equal("https://github.com/org/care-call/pull/42", message.ButtonUrl);
    }

    [Fact]
    public void BuildMessage_uses_merged_date_for_merged_pull_request()
    {
        var formatter = new PullRequestNotificationFormatter();
        var record = CreateRecord(PullRequestStatus.Merged);
        record.MergedAt = new DateTimeOffset(2026, 5, 2, 17, 30, 0, TimeSpan.Zero);

        var message = formatter.BuildMessage(record);

        Assert.Contains("✅ Merged", message.Text);
        Assert.Contains("02-05-2026 20:30:00", message.Text);
    }

    [Fact]
    public void BuildMessage_uses_closed_date_for_closed_pull_request()
    {
        var formatter = new PullRequestNotificationFormatter();
        var record = CreateRecord(PullRequestStatus.Closed);
        record.ClosedAt = new DateTimeOffset(2026, 5, 2, 18, 45, 0, TimeSpan.Zero);

        var message = formatter.BuildMessage(record);

        Assert.Contains("❌ Closed", message.Text);
        Assert.Contains("02-05-2026 21:45:00", message.Text);
    }

    private static PullRequestRecord CreateRecord(PullRequestStatus status)
    {
        return new PullRequestRecord
        {
            Author = "octocat",
            RepoOwner = "org",
            RepoName = "care-call",
            PrNumber = 42,
            PrTitle = "CCL-214 Add RateLimiter",
            PrCreateAt = new DateTimeOffset(2026, 5, 2, 16, 19, 58, TimeSpan.Zero),
            UpdatedAt = new DateTimeOffset(2026, 5, 2, 16, 20, 0, TimeSpan.Zero),
            PrStatus = status,
            RepoUrl = "https://github.com/org/care-call",
            PrUrl = "https://github.com/org/care-call/pull/42"
        };
    }
}
