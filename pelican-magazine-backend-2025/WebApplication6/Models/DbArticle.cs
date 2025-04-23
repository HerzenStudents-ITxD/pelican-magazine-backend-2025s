using System.ComponentModel.DataAnnotations.Schema;
using Backend.Contracts.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Models;

public class DbArticle
{
    public const string TableName = "Articles";

    public Guid ArticleId { get; set; } = Guid.NewGuid();
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Text { get; set; }
    public string? Thumbnail { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public ArticleStatus Status { get; set; } = ArticleStatus.Draft;

    public required Guid AuthorId { get; set; }
    public required DbUser Author { get; set; }

    [NotMapped]
    public ICollection<DbArticleAgeCategory> ArticleAgeCategories { get; set; } = new List<DbArticleAgeCategory>();
    [NotMapped]
    public ICollection<DbArticleTheme> ArticleThemes { get; set; } = new List<DbArticleTheme>();
    [NotMapped]
    public ICollection<DbTitle> Titles { get; set; } = new List<DbTitle>(); 
    [NotMapped]
    public ICollection<DbArticleReview> Reviews { get; set; } = new List<DbArticleReview>();
}

public class DbArticleConfiguration : IEntityTypeConfiguration<DbArticle>
{
    public void Configure(EntityTypeBuilder<DbArticle> builder)
    {
        builder.ToTable(DbArticle.TableName);
        builder.HasKey(a => a.ArticleId);

        builder.HasMany(a => a.ArticleAgeCategories)
            .WithOne(aac => aac.Article)
            .HasForeignKey(aac => aac.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.ArticleThemes)
            .WithOne(at => at.Article)
            .HasForeignKey(at => at.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Titles)
            .WithOne(t => t.Article)
            .HasForeignKey(t => t.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Reviews)
            .WithOne(r => r.Article)
            .HasForeignKey(r => r.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}