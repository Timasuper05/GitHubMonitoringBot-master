using Octokit;
using Microsoft.Extensions.Logging;

namespace GitHub_Monitoring_Bot;

public class GitHubPullRequestService(
    AppSettings settings,
    GitHubClient gitClient,
    ILogger<GitHubPullRequestService> logger)
{
    public async Task<IReadOnlyList<PullRequest>> GetRepositoryPullRequestsAsync(
        string owner,
        string repositoryName)
    {
        var request = new PullRequestRequest
        {
            State = ItemStateFilter.Open
        };

        logger.LogInformation("Getting open pull requests for {Owner}/{RepositoryName}", owner, repositoryName);
        var pullRequests = await gitClient.PullRequest.GetAllForRepository(owner, repositoryName, request);
        logger.LogInformation(
            "GitHub returned {PullRequestCount} open pull requests for {Owner}/{RepositoryName}",
            pullRequests.Count,
            owner,
            repositoryName);

        return pullRequests;
    }

    public async Task<PullRequest> GetRepositoryPullRequestAsync(
        string owner,
        string repositoryName,
        int pullRequestNumber)
    {
        logger.LogInformation(
            "Getting pull request {Owner}/{RepositoryName} #{PullRequestNumber}",
            owner,
            repositoryName,
            pullRequestNumber);

        var pullRequest = await gitClient.PullRequest.Get(owner, repositoryName, pullRequestNumber);
        logger.LogInformation(
            "GitHub returned pull request {Owner}/{RepositoryName} #{PullRequestNumber} with state {State}",
            owner,
            repositoryName,
            pullRequestNumber,
            pullRequest.State.Value);

        return pullRequest;
    }

    public async Task<List<Repository>> GetConfiguredRepositoriesAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        List<Repository> trackedRepositories = [];
        foreach (var trackedRepo in settings.RepoList)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(trackedRepo.RepoName))
            {
                logger.LogWarning("Repository config item was skipped because RepoName is empty");
                continue;
            }

            if (string.IsNullOrWhiteSpace(trackedRepo.Owner))
            {
                logger.LogWarning(
                    "Repository config item {RepositoryName} was skipped because Owner is empty",
                    trackedRepo.RepoName);
                continue;
            }

            var repo = await GetConfiguredRepositoryAsync(trackedRepo, cancellationToken);
            if (repo != null)
            {
                trackedRepositories.Add(repo);
            }
        }

        return trackedRepositories
            .DistinctBy(x => x.FullName, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private async Task<Repository?> GetConfiguredRepositoryAsync(
        RepositoryConfig trackedRepo,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation(
                "Getting configured repository {Owner}/{RepositoryName}",
                trackedRepo.Owner,
                trackedRepo.RepoName);

            var repo = await gitClient.Repository.Get(trackedRepo.Owner, trackedRepo.RepoName);
            cancellationToken.ThrowIfCancellationRequested();

            if (!IsConfiguredRepository(repo, trackedRepo))
            {
                logger.LogWarning(
                    "GitHub returned repository {ActualOwner}/{ActualRepositoryName}, but config expects {ExpectedOwner}/{ExpectedRepositoryName}",
                    repo.Owner.Login,
                    repo.Name,
                    trackedRepo.Owner,
                    trackedRepo.RepoName);

                return null;
            }

            logger.LogInformation("GitHub found configured repository {Owner}/{RepositoryName}", repo.Owner.Login, repo.Name);
            return repo;
        }
        catch (NotFoundException)
        {
            logger.LogWarning(
                "GitHub repository {Owner}/{RepositoryName} was not found or token has no access",
                trackedRepo.Owner,
                trackedRepo.RepoName);

            return null;
        }
    }

    private static bool IsConfiguredRepository(Repository repo, RepositoryConfig trackedRepo)
    {
        return string.Equals(repo.Name, trackedRepo.RepoName, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(repo.Owner.Login, trackedRepo.Owner, StringComparison.OrdinalIgnoreCase);
    }
}
