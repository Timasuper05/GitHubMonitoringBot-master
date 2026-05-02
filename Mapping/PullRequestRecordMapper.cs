using MapsterMapper;
using Octokit;

namespace GitHub_Monitoring_Bot;

public class PullRequestRecordMapper(IMapper mapper)
{
    public PullRequestRecord Create(Repository repository, PullRequest pullRequest, PullRequestStatus status)
    {
        var pullRequestRecord = mapper.Map<PullRequestRecord>(pullRequest);
        MapAdditionalFields(pullRequestRecord, repository, status);

        return pullRequestRecord;
    }

    public void Update(
        PullRequestRecord pullRequestRecord,
        Repository repository,
        PullRequest pullRequest,
        PullRequestStatus status)
    {
        mapper.Map(pullRequest, pullRequestRecord);
        MapAdditionalFields(pullRequestRecord, repository, status);
    }

    private static void MapAdditionalFields(
        PullRequestRecord pullRequestRecord,
        Repository repository,
        PullRequestStatus status)
    {
        pullRequestRecord.RepoOwner = repository.Owner.Login;
        pullRequestRecord.RepoName = repository.Name;
        pullRequestRecord.RepoUrl = repository.HtmlUrl;
        pullRequestRecord.PrStatus = status;
    }
}
