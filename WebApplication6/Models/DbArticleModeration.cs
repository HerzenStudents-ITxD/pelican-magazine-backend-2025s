using System.ComponentModel.DataAnnotations.Schema;
using Backend.Contracts.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Models;

public class DbArticleModeration
{
    public const string TableName = "ArticleModerations";

    public Guid ModerationId { get; set; } = Guid.NewGuid();
    public Guid ArticleId { get; set; }
    public DbArticle Article { get; set; }
    public Guid ModeratorId { get; set; }
    public DbUser Moderator { get; set; }
    public DateTime ModerationDate { get; set; } = DateTime.UtcNow;
    public ModerationStatus Status { get; set; }
    public string? Comment { get; set; }
    public string? RejectionReasons { get; set; } // JSON с выделенными словами/фразами
}

public class DbArticleModerationConfiguration : IEntityTypeConfiguration<DbArticleModeration>
{
    public void Configure(EntityTypeBuilder<DbArticleModeration> builder)
    {
        builder.ToTable(DbArticleModeration.TableName);
        builder.HasKey(m => m.ModerationId);

        builder.HasOne(m => m.Article)
            .WithMany()
            .HasForeignKey(m => m.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.Moderator)
            .WithMany()
            .HasForeignKey(m => m.ModeratorId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(m => m.Status)
            .HasConversion<string>()
            .HasMaxLength(20);
    }
}

public enum ModerationStatus
{
    Pending,
    Approved,
    Rejected,
    NeedsRevision
}