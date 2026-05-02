using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GitHub_Monitoring_Bot.Migrations;

[DbContext(typeof(DataContext))]
partial class DataContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "10.0.6");

        modelBuilder.Entity("GitHub_Monitoring_Bot.PullRequestRecord", b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("INTEGER");

            b.Property<string>("Author")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<DateTimeOffset?>("ClosedAt")
                .HasColumnType("TEXT");

            b.Property<long>("GitHubPrId")
                .HasColumnType("INTEGER");

            b.Property<DateTimeOffset?>("MergedAt")
                .HasColumnType("TEXT");

            b.Property<DateTimeOffset>("PrCreateAt")
                .HasColumnType("TEXT");

            b.Property<string>("PrDescription")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<int>("PrNumber")
                .HasColumnType("INTEGER");

            b.Property<int>("PrStatus")
                .HasColumnType("INTEGER");

            b.Property<string>("PrTitle")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<string>("PrUrl")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<string>("RepoName")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<string>("RepoOwner")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<string>("RepoUrl")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<DateTimeOffset>("UpdatedAt")
                .HasColumnType("TEXT");

            b.HasKey("Id");

            b.HasIndex("RepoOwner", "RepoName", "PrNumber")
                .IsUnique();

            b.ToTable("PullRequests");
        });
    }
}
