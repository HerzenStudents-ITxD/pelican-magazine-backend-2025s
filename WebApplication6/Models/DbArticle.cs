using System.ComponentModel.DataAnnotations.Schema;
using Backend.Contracts.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Backend.DataAccess;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class DbArticle
{
    public const string TableName = "Articles";

    public Guid ArticleId { get; set; } = Guid.NewGuid();
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Text { get; set; }
    public string? Summary { get; set; }
    public string? Thumbnail { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public ArticleStatus Status { get; set; } = ArticleStatus.Draft;
    [Required]
    public Guid AuthorId { get; set; }

    public DbUser? Author { get; set; }
    

    


    // Навигационные свойства
    public ICollection<DbArticleAgeCategory> ArticleAgeCategories { get; set; } = new List<DbArticleAgeCategory>();
    public ICollection<DbLike> Likes { get; set; } = new List<DbLike>();
    public ICollection<DbArticleTheme> ArticleThemes { get; set; } = new List<DbArticleTheme>();

    public ICollection<DbArticleReview> Reviews { get; set; } = new List<DbArticleReview>();
    
}

public class DbArticleConfiguration : IEntityTypeConfiguration<DbArticle>
{
    public void Configure(EntityTypeBuilder<DbArticle> builder)
    {
        builder.ToTable(DbArticle.TableName);
        builder.HasKey(a => a.ArticleId);

        builder.Property(a => a.Title).IsRequired().HasMaxLength(200);
        builder.Property(a => a.Description).IsRequired().HasMaxLength(500);
        builder.Property(a => a.Text).IsRequired();
        builder.Property(a => a.ChangedAt).IsRequired();

        builder.Property(a => a.Status)
        .IsRequired()
        .HasConversion(
        v => v.ToString(),
        v => (ArticleStatus)Enum.Parse(typeof(ArticleStatus), v));

        builder.HasOne(a => a.Author)
            .WithMany(u => u.Articles)
            .HasForeignKey(a => a.AuthorId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(a => a.Reviews)
            .WithOne(r => r.Article)
            .HasForeignKey(r => r.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

