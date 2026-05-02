using Microsoft.EntityFrameworkCore;

namespace GitHub_Monitoring_Bot;

public class PullRequestRecordRepository(DataContext dataContext)
{
    public async Task<PullRequestRecord?> GetByRepositoryAndNumberAsync(
        string repositoryOwner,
        string repositoryName,
        int pullRequestNumber,
        CancellationToken cancellationToken)
    {
        return await dataContext.PullRequests.FirstOrDefaultAsync(x =>
            x.RepoOwner == repositoryOwner &&
            x.RepoName == repositoryName &&
            x.PrNumber == pullRequestNumber, cancellationToken);
    }

    public async Task<List<PullRequestRecord>> GetByRepositoryAsync(
        string repositoryOwner,
        string repositoryName,
        CancellationToken cancellationToken)
    {
        return await dataContext.PullRequests
            .Where(x => x.RepoOwner == repositoryOwner && x.RepoName == repositoryName)
            .ToListAsync(cancellationToken);
    }

    public void Add(PullRequestRecord pullRequestRecord)
    {
        dataContext.PullRequests.Add(pullRequestRecord);
    }

    public void RemoveRange(IEnumerable<PullRequestRecord> pullRequestRecords)
    {
        dataContext.PullRequests.RemoveRange(pullRequestRecords);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await dataContext.SaveChangesAsync(cancellationToken);
    }
}
