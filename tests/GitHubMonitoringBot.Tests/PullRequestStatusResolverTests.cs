using Octokit;
using Xunit;

namespace GitHub_Monitoring_Bot.Tests;

public class PullRequestStatusResolverTests
{
    private readonly PullRequestStatusResolver resolver = new();

    [Fact]
    public void Resolve_returns_open_for_open_pull_request()
    {
        var status = resolver.Resolve(ItemState.Open, mergedAt: null);

        Assert.Equal(PullRequestStatus.Open, status);
    }

    [Fact]
    public void Resolve_returns_merged_for_closed_pull_request_with_merged_date()
    {
        var status = resolver.Resolve(ItemState.Closed, DateTimeOffset.UtcNow);

        Assert.Equal(PullRequestStatus.Merged, status);
    }

    [Fact]
    public void Resolve_returns_closed_for_closed_pull_request_without_merged_date()
    {
        var status = resolver.Resolve(ItemState.Closed, mergedAt: null);

        Assert.Equal(PullRequestStatus.Closed, status);
    }
}
