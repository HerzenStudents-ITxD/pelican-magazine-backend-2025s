using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

public class DbArticleAgeCategory
{
    public const string TableName = "Article_AgeCategories";

    public Guid ArticleAgeId { get; set; } = Guid.NewGuid();
    public Guid AgeCategoryId { get; set; }
    public Guid ArticleId { get; set; }
    public string CategoryName { get; set; }

    [ForeignKey("AgeCategoryId")]
    public DbAgeCategory AgeCategory { get; set; }

    [ForeignKey("ArticleId")]
    public DbArticle Article { get; set; }
}

public class DbArticleAgeCategoryConfiguration : IEntityTypeConfiguration<DbArticleAgeCategory>
{
    public void Configure(EntityTypeBuilder<DbArticleAgeCategory> builder)
    {
        builder.ToTable(DbArticleAgeCategory.TableName);
        builder.HasKey(aac => aac.ArticleAgeId);

        builder.HasOne(aac => aac.AgeCategory)
            .WithMany(ac => ac.ArticleAgeCategories)
            .HasForeignKey(aac => aac.AgeCategoryId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(aac => aac.Article)
            .WithMany(a => a.ArticleAgeCategories)
            .HasForeignKey(aac => aac.ArticleId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
