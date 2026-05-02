namespace GitHub_Monitoring_Bot;

public class AppSettings
{
    public string BotToken { get; set; } = string.Empty;
    public string GitPat { get; set; } = string.Empty;
    public string ChatId { get; set; } = string.Empty;
    public int? ThreadId { get; set; }
    public int Interval { get; set; }
    public List<RepositoryConfig> RepoList { get; set; } = [];
}
