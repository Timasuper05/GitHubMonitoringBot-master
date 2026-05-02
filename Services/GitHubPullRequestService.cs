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
                var repositories = await FindRepositoriesByNameAsync(trackedRepo.RepoName, cancellationToken);
                trackedRepositories.AddRange(repositories);

                continue;
            }

            try
            {
                var repo = await gitClient.Repository.Get(trackedRepo.Owner, trackedRepo.RepoName);
                trackedRepositories.Add(repo);
                logger.LogInformation("GitHub found configured repository {Owner}/{RepositoryName}", repo.Owner.Login, repo.Name);
            }
            catch (NotFoundException)
            {
                logger.LogWarning(
                    "GitHub repository {Owner}/{RepositoryName} was not found or token has no access",
                    trackedRepo.Owner,
                    trackedRepo.RepoName);
            }
        }

        return trackedRepositories
            .DistinctBy(x => x.FullName, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private async Task<List<Repository>> FindRepositoriesByNameAsync(
        string repositoryName,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Owner is not configured for {RepositoryName}; searching accessible repositories by name", repositoryName);

        var repositories = await gitClient.Repository.GetAllForCurrent();
        cancellationToken.ThrowIfCancellationRequested();

        var matchedRepositories = repositories
            .Where(x => string.Equals(x.Name, repositoryName, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (matchedRepositories.Count == 0)
        {
            logger.LogWarning(
                "GitHub did not find accessible repositories with name {RepositoryName}. Configure Owner if this is an organization repository",
                repositoryName);

            return [];
        }

        foreach (var repository in matchedRepositories)
        {
            logger.LogInformation("GitHub found configured repository {Owner}/{RepositoryName}", repository.Owner.Login, repository.Name);
        }

        return matchedRepositories;
    }
}
