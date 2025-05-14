using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Models;

public class DbArticleReview
{
    public const string TableName = "Article_Reviews";

    public Guid ReviewId { get; set; }
    public Guid ArticleId { get; set; }
    public DbArticle Article { get; set; }
    public Guid UserId { get; set; }
    public DbUser User { get; set; }
    public DateTime ReviewDate { get; set; }
    public string Comments { get; set; }
}

public class DbArticleReviewConfiguration : IEntityTypeConfiguration<DbArticleReview>
{
    public void Configure(EntityTypeBuilder<DbArticleReview> builder)
    {
        builder.ToTable(DbArticleReview.TableName);
        builder.HasKey(r => r.ReviewId);

        builder.HasOne(r => r.Article)
            .WithMany(a => a.Reviews)
            .HasForeignKey(r => r.ArticleId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(r => r.User)
            .WithMany(u => u.ArticleReviews)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}