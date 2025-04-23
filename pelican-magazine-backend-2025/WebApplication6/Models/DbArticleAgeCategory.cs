using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Models;

public class DbArticleAgeCategory
{
    public const string TableName = "Article_AgeCategories";

    public int ArticleAgeId { get; set; }
    public int AgeCategoryId { get; set; }
    public DbAgeCategory AgeCategory { get; set; }
    public Guid ArticleId { get; set; }
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
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(aac => aac.Article)
            .WithMany(a => a.ArticleAgeCategories)
            .HasForeignKey(aac => aac.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
