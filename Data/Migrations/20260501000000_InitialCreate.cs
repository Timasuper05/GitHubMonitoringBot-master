using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GitHub_Monitoring_Bot.Migrations;

[DbContext(typeof(DataContext))]
[Migration("20260501000000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "PullRequests",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Author = table.Column<string>(type: "TEXT", nullable: false),
                RepoName = table.Column<string>(type: "TEXT", nullable: false),
                PrNumber = table.Column<int>(type: "INTEGER", nullable: false),
                GitHubPrId = table.Column<long>(type: "INTEGER", nullable: false),
                PrTitle = table.Column<string>(type: "TEXT", nullable: false),
                PrDescription = table.Column<string>(type: "TEXT", nullable: false),
                PrCreateAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                ClosedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                MergedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                PrStatus = table.Column<int>(type: "INTEGER", nullable: false),
                PrUrl = table.Column<string>(type: "TEXT", nullable: false),
                RepoUrl = table.Column<string>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PullRequests", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_PullRequests_RepoName_PrNumber",
            table: "PullRequests",
            columns: new[] { "RepoName", "PrNumber" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "PullRequests");
    }
}
