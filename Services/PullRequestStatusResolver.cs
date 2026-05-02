using Octokit;

namespace GitHub_Monitoring_Bot;

public class PullRequestStatusResolver
{
    public PullRequestStatus Resolve(PullRequest pullRequest)
    {
        return Resolve(pullRequest.State, pullRequest.MergedAt);
    }

    public PullRequestStatus Resolve(StringEnum<ItemState> state, DateTimeOffset? mergedAt)
    {
        if (state == ItemState.Open)
        {
            return PullRequestStatus.Open;
        }

        if (state == ItemState.Closed && mergedAt != null)
        {
            return PullRequestStatus.Merged;
        }

        if (state == ItemState.Closed)
        {
            return PullRequestStatus.Closed;
        }

        return PullRequestStatus.Open;
    }
}
