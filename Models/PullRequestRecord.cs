namespace GitHub_Monitoring_Bot;

public class PullRequestRecord
{
    public int Id { get; set; }
    public string Author { get; set; } = string.Empty;
    public string RepoOwner { get; set; } = string.Empty;
    public string RepoName { get; set; } = string.Empty;
    public int PrNumber { get; set; }
    public long GitHubPrId { get; set; }
    public string PrTitle { get; set; } = string.Empty;
    public string PrDescription { get; set; } = string.Empty;
    public DateTimeOffset PrCreateAt { get; set; }
    public DateTimeOffset? ClosedAt { get; set; }
    public DateTimeOffset? MergedAt { get; set; }
    public PullRequestStatus PrStatus { get; set; }
    public string PrUrl { get; set; } = string.Empty;
    public string RepoUrl { get; set; } = string.Empty;
    public DateTimeOffset UpdatedAt { get; set; }
}
