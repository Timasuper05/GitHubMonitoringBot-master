using GitHub_Monitoring_Bot.Mapping;
using Mapster;
using MapsterMapper;
using Octokit;
using Xunit;

namespace GitHub_Monitoring_Bot.Tests;

public class PullRequestRecordMapperTests
{
    [Fact]
    public void Create_maps_pull_request_and_repository_fields()
    {
        var mapper = CreateMapper();
        var repository = CreateRepository("care-call", "https://github.com/org/care-call");
        var pullRequest = CreatePullRequest(
            id: 1001,
            number: 42,
            title: "Add monitoring",
            body: null,
            htmlUrl: "https://github.com/org/care-call/pull/42",
            authorLogin: "octocat",
            state: ItemState.Open,
            mergedAt: null);

        var record = mapper.Create(repository, pullRequest, PullRequestStatus.Open);

        Assert.Equal("care-call", record.RepoName);
        Assert.Equal("org", record.RepoOwner);
        Assert.Equal("https://github.com/org/care-call", record.RepoUrl);
        Assert.Equal(1001, record.GitHubPrId);
        Assert.Equal(42, record.PrNumber);
        Assert.Equal("Add monitoring", record.PrTitle);
        Assert.Equal(string.Empty, record.PrDescription);
        Assert.Equal("octocat", record.Author);
        Assert.Equal("https://github.com/org/care-call/pull/42", record.PrUrl);
        Assert.Equal(PullRequestStatus.Open, record.PrStatus);
    }

    [Fact]
    public void Update_refreshes_existing_record()
    {
        var mapper = CreateMapper();
        var repository = CreateRepository("care-call", "https://github.com/org/care-call");
        var pullRequest = CreatePullRequest(
            id: 1001,
            number: 42,
            title: "Updated title",
            body: "Updated body",
            htmlUrl: "https://github.com/org/care-call/pull/42",
            authorLogin: "octocat",
            state: ItemState.Closed,
            mergedAt: DateTimeOffset.UtcNow);
        var record = new PullRequestRecord
        {
            RepoName = "old",
            PrTitle = "old",
            PrStatus = PullRequestStatus.Open
        };

        mapper.Update(record, repository, pullRequest, PullRequestStatus.Merged);

        Assert.Equal("care-call", record.RepoName);
        Assert.Equal("Updated title", record.PrTitle);
        Assert.Equal("Updated body", record.PrDescription);
        Assert.Equal(PullRequestStatus.Merged, record.PrStatus);
    }

    private static PullRequestRecordMapper CreateMapper()
    {
        var config = new TypeAdapterConfig();
        new MappingConfig().Register(config);
        var mapper = new Mapper(config);

        return new PullRequestRecordMapper(mapper);
    }

    private static Repository CreateRepository(string name, string htmlUrl)
    {
        var now = DateTimeOffset.UtcNow;

        return new Repository(
            url: string.Empty,
            htmlUrl: htmlUrl,
            cloneUrl: string.Empty,
            gitUrl: string.Empty,
            sshUrl: string.Empty,
            svnUrl: string.Empty,
            mirrorUrl: string.Empty,
            archiveUrl: string.Empty,
            id: 1,
            nodeId: string.Empty,
            owner: CreateUser("org"),
            name: name,
            fullName: $"org/{name}",
            isTemplate: false,
            description: string.Empty,
            homepage: string.Empty,
            language: "C#",
            @private: false,
            fork: false,
            forksCount: 0,
            stargazersCount: 0,
            defaultBranch: "main",
            openIssuesCount: 0,
            pushedAt: now,
            createdAt: now,
            updatedAt: now,
            permissions: null!,
            parent: null!,
            source: null!,
            license: null!,
            hasDiscussions: false,
            hasIssues: true,
            hasWiki: false,
            hasDownloads: true,
            hasPages: false,
            subscribersCount: 0,
            size: 0,
            allowRebaseMerge: null,
            allowSquashMerge: null,
            allowMergeCommit: null,
            archived: false,
            watchersCount: 0,
            deleteBranchOnMerge: null,
            visibility: RepositoryVisibility.Public,
            topics: Array.Empty<string>(),
            allowAutoMerge: null,
            allowUpdateBranch: null,
            webCommitSignoffRequired: null,
            securityAndAnalysis: null!);
    }

    private static PullRequest CreatePullRequest(
        long id,
        int number,
        string title,
        string? body,
        string htmlUrl,
        string authorLogin,
        ItemState state,
        DateTimeOffset? mergedAt)
    {
        var now = DateTimeOffset.UtcNow;

        return new PullRequest(
            id: id,
            nodeId: string.Empty,
            url: string.Empty,
            htmlUrl: htmlUrl,
            diffUrl: string.Empty,
            patchUrl: string.Empty,
            issueUrl: string.Empty,
            statusesUrl: string.Empty,
            number: number,
            state: state,
            title: title,
            body: body,
            createdAt: now,
            updatedAt: now,
            closedAt: state == ItemState.Closed ? now : null,
            mergedAt: mergedAt,
            head: null!,
            @base: null!,
            user: CreateUser(authorLogin),
            assignee: null!,
            assignees: Array.Empty<User>(),
            draft: false,
            mergeable: null,
            mergeableState: null,
            mergedBy: null!,
            mergeCommitSha: string.Empty,
            comments: 0,
            commits: 0,
            additions: 0,
            deletions: 0,
            changedFiles: 0,
            milestone: null!,
            locked: false,
            maintainerCanModify: null,
            requestedReviewers: Array.Empty<User>(),
            requestedTeams: Array.Empty<Team>(),
            labels: Array.Empty<Label>(),
            activeLockReason: null);
    }

    private static User CreateUser(string login)
    {
        var now = DateTimeOffset.UtcNow;

        return new User(
            avatarUrl: string.Empty,
            bio: string.Empty,
            blog: string.Empty,
            collaborators: 0,
            company: string.Empty,
            createdAt: now,
            updatedAt: now,
            diskUsage: 0,
            email: string.Empty,
            followers: 0,
            following: 0,
            hireable: null,
            htmlUrl: string.Empty,
            totalPrivateRepos: 0,
            id: 1,
            location: string.Empty,
            login: login,
            name: login,
            nodeId: string.Empty,
            ownedPrivateRepos: 0,
            plan: null!,
            privateGists: 0,
            publicGists: 0,
            publicRepos: 0,
            url: string.Empty,
            permissions: null!,
            siteAdmin: false,
            ldapDistinguishedName: string.Empty,
            suspendedAt: null);
    }
}
