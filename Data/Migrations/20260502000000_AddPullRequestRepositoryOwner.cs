using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GitHub_Monitoring_Bot.Migrations;

[DbContext(typeof(DataContext))]
[Migration("20260502000000_AddPullRequestRepositoryOwner")]
public partial class AddPullRequestRepositoryOwner : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_PullRequests_RepoName_PrNumber",
            table: "PullRequests");

        migrationBuilder.AddColumn<string>(
            name: "RepoOwner",
            table: "PullRequests",
            type: "TEXT",
            nullable: false,
            defaultValue: string.Empty);

        migrationBuilder.CreateIndex(
            name: "IX_PullRequests_RepoOwner_RepoName_PrNumber",
            table: "PullRequests",
            columns: new[] { "RepoOwner", "RepoName", "PrNumber" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_PullRequests_RepoOwner_RepoName_PrNumber",
            table: "PullRequests");

        migrationBuilder.DropColumn(
            name: "RepoOwner",
            table: "PullRequests");

        migrationBuilder.CreateIndex(
            name: "IX_PullRequests_RepoName_PrNumber",
            table: "PullRequests",
            columns: new[] { "RepoName", "PrNumber" },
            unique: true);
    }
}
