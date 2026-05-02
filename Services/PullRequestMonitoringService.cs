using Microsoft.Extensions.Logging;

namespace GitHub_Monitoring_Bot;

public class PullRequestMonitoringService(
    GitHubPullRequestService gitHubPullRequestService,
    PullRequestNotificationService pullRequestNotificationService,
    PullRequestRecordRepository pullRequestRecordRepository,
    PullRequestRecordMapper pullRequestRecordMapper,
    PullRequestStatusResolver pullRequestStatusResolver,
    ILogger<PullRequestMonitoringService> logger)
{
    public async Task CheckTrackedRepositoriesPullRequestsAsync(CancellationToken cancellationToken)
    {
        var repositories = await gitHubPullRequestService.GetConfiguredRepositoriesAsync(cancellationToken);
        if (repositories.Count == 0)
        {
            logger.LogWarning("No configured repositories were found for monitoring");
            return;
        }

        logger.LogInformation("Monitoring {RepositoryCount} configured repositories", repositories.Count);

        foreach (var repo in repositories)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var prs = await gitHubPullRequestService.GetRepositoryPullRequestsAsync(repo.Owner.Login, repo.Name);
            var openPullRequestNumbers = prs.Select(x => x.Number).ToHashSet();
            foreach (var pr in prs)
            {
                var status = pullRequestStatusResolver.Resolve(pr);

                var existingPr = await pullRequestRecordRepository.GetByRepositoryAndNumberAsync(
                    repo.Owner.Login,
                    repo.Name,
                    pr.Number,
                    cancellationToken);

                if (existingPr == null)
                {
                    var pullRequestRecord = pullRequestRecordMapper.Create(repo, pr, status);
                    pullRequestRecordRepository.Add(pullRequestRecord);
                    logger.LogInformation(
                        "Started tracking pull request {Owner}/{RepositoryName} #{PullRequestNumber}",
                        repo.Owner.Login,
                        repo.Name,
                        pr.Number);
                    await pullRequestNotificationService.SendPullRequestNotificationAsync(
                        pullRequestRecord,
                        cancellationToken);
                }
                else if (existingPr.PrStatus != status)
                {
                    var previousStatus = existingPr.PrStatus;
                    pullRequestRecordMapper.Update(existingPr, repo, pr, status);
                    logger.LogInformation(
                        "Pull request {Owner}/{RepositoryName} #{PullRequestNumber} status changed from {PreviousStatus} to {CurrentStatus}",
                        repo.Owner.Login,
                        repo.Name,
                        pr.Number,
                        previousStatus,
                        status);
                    await pullRequestNotificationService.SendPullRequestNotificationAsync(
                        existingPr,
                        cancellationToken);
                }
                else
                {
                    pullRequestRecordMapper.Update(existingPr, repo, pr, status);
                }
            }

            var trackedPullRequests = await pullRequestRecordRepository.GetByRepositoryAsync(
                repo.Owner.Login,
                repo.Name,
                cancellationToken);
            var inactivePullRequests = trackedPullRequests
                .Where(x => !openPullRequestNumbers.Contains(x.PrNumber))
                .ToList();

            if (inactivePullRequests.Count > 0)
            {
                foreach (var inactivePullRequest in inactivePullRequests)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var pullRequest = await gitHubPullRequestService.GetRepositoryPullRequestAsync(
                        repo.Owner.Login,
                        repo.Name,
                        inactivePullRequest.PrNumber);
                    var status = pullRequestStatusResolver.Resolve(pullRequest);
                    var previousStatus = inactivePullRequest.PrStatus;

                    pullRequestRecordMapper.Update(inactivePullRequest, repo, pullRequest, status);
                    if (previousStatus != status)
                    {
                        logger.LogInformation(
                            "Pull request {Owner}/{RepositoryName} #{PullRequestNumber} status changed from {PreviousStatus} to {CurrentStatus}",
                            repo.Owner.Login,
                            repo.Name,
                            inactivePullRequest.PrNumber,
                            previousStatus,
                            status);
                        await pullRequestNotificationService.SendPullRequestNotificationAsync(
                            inactivePullRequest,
                            cancellationToken);
                    }
                }

                pullRequestRecordRepository.RemoveRange(inactivePullRequests);
                logger.LogInformation(
                    "Stopped tracking {PullRequestCount} pull requests for {Owner}/{RepositoryName} after they left the open state",
                    inactivePullRequests.Count,
                    repo.Owner.Login,
                    repo.Name);
            }
        }

        await pullRequestRecordRepository.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Pull request monitoring state was saved");
    }
}
