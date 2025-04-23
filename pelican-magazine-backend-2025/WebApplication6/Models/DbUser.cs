using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Backend.Models;


public class DbUser
{
    public const string TableName = "Users";

    public Guid UserId { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public required string LastName { get; set; }
    public string? Nickname { get; set; }
    public required string PasswordHash { get; set; }
    public required string Email { get; set; }
    public required string Sec { get; set; } = Guid.NewGuid().ToString();
    public DateTime Birth { get; set; }
    public string? ProfileImg { get; set; }
    public string? ProfileCover { get; set; }
    public bool IsAdmin { get; set; }

    [NotMapped]
    public ICollection<DbArticle> Articles { get; set; } = new List<DbArticle>();
    [NotMapped]
    public ICollection<DbArticleReview> ArticleReviews { get; set; } = new List<DbArticleReview>();

    [NotMapped]
    public ICollection<DbTitle> Titles { get; set; } = new List<DbTitle>();
}

public class DbUserConfiguration : IEntityTypeConfiguration<DbUser>
{
    public void Configure(EntityTypeBuilder<DbUser> builder)
    {
        builder.ToTable(DbUser.TableName);
        builder.HasKey(u => u.UserId);

        builder.HasMany(u => u.Articles)
            .WithOne(a => a.Author)
            .HasForeignKey(a => a.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.ArticleReviews)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Titles)
            .WithOne(t => t.User)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}