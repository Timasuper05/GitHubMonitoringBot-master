using Mapster;
using Octokit;

namespace GitHub_Monitoring_Bot.Mapping;

public class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<PullRequest, PullRequestRecord>()
            .Map(dest => dest.Author, src => src.User.Login)
            .Map(dest => dest.GitHubPrId, src => src.Id)
            .Map(dest => dest.PrNumber, src => src.Number)
            .Map(dest => dest.PrTitle, src => src.Title)
            .Map(dest => dest.PrDescription, src => src.Body ?? string.Empty)
            .Map(dest => dest.PrCreateAt, src => src.CreatedAt)
            .Map(dest => dest.ClosedAt, src => src.ClosedAt)
            .Map(dest => dest.MergedAt, src => src.MergedAt)
            .Map(dest => dest.PrUrl, src => src.HtmlUrl)
            .Map(dest => dest.UpdatedAt, src => src.UpdatedAt);
    }
}
