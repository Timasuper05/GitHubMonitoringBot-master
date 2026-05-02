
using Microsoft.EntityFrameworkCore;

namespace GitHub_Monitoring_Bot;

public class DataContext : DbContext
{
    public DbSet<PullRequestRecord> PullRequests { get; set; } = null!;
    
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.UseSqlite("Data Source=github-monitoring.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PullRequestRecord>()
            .HasIndex(x => new { x.RepoOwner, x.RepoName, x.PrNumber })
            .IsUnique();
    }
}
