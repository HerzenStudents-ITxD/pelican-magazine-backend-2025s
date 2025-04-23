using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Models;

public class DbTitle
{
    public const string TableName = "Titles";

    public int TitleId { get; set; }
    public Guid UserId { get; set; }
    public DbUser User { get; set; }
    public Guid ArticleId { get; set; }
    public DbArticle Article { get; set; }
}

public class DbTitleConfiguration : IEntityTypeConfiguration<DbTitle>
{
    public void Configure(EntityTypeBuilder<DbTitle> builder)
    {
        builder.ToTable(DbTitle.TableName);
        builder.HasKey(t => t.TitleId);

        builder.HasOne(t => t.User)
            .WithMany(u => u.Titles)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Article)
            .WithMany(a => a.Titles)
            .HasForeignKey(t => t.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}