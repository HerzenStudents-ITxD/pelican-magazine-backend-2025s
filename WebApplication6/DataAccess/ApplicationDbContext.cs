using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Backend.DataAccess;
using static Backend.Models.DbUser;


namespace Backend.DataAccess;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<DbUser> Users { get; set; }
    public DbSet<DbArticle> Articles { get; set; }
    public DbSet<DbAgeCategory> AgeCategories { get; set; }
    public DbSet<DbArticleAgeCategory> ArticleAgeCategories { get; set; }
    public DbSet<DbTheme> Themes { get; set; }
    public DbSet<DbArticleTheme> ArticleThemes { get; set; }
    public DbSet<DbLike> Likes { get; set; }
    public DbSet<DbArticleReview> ArticleReviews { get; set; }
    public DbSet<DbArticleModeration> ArticleModerations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new DbArticleModerationConfiguration());
        modelBuilder.Entity<DbUser>(entity =>
        {
            entity.Property(u => u.Nickname).HasMaxLength(50);
            entity.Property(u => u.ProfileImg).HasMaxLength(255);
            entity.Property(u => u.ProfileCover).HasMaxLength(255);
            entity.Property(u => u.Degree).HasMaxLength(100);
            entity.Property(u => u.UserType)
                  .HasConversion<string>()
                  .HasMaxLength(20);

            entity.Property(e => e.EmailVerificationToken).HasMaxLength(64);
            entity.Property(e => e.EmailVerificationTokenExpiry);
            entity.Property(e => e.EmailVerified).HasDefaultValue(false);
            entity.Property(e => e.TwoFactorEnabled).HasDefaultValue(false);
            entity.Property(e => e.TwoFactorSecret).HasMaxLength(32);
        });
        modelBuilder.Entity<DbLike>(entity =>
        {
            entity.HasKey(l => l.LikeId);
            entity.HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UserId);
            entity.HasOne(l => l.Article)
                .WithMany(a => a.Likes)
                .HasForeignKey(l => l.ArticleId);
        });

        modelBuilder.ApplyConfiguration(new DbUserConfiguration());
        modelBuilder.ApplyConfiguration(new DbArticleConfiguration());
        modelBuilder.ApplyConfiguration(new DbAgeCategoryConfiguration());
        modelBuilder.ApplyConfiguration(new DbArticleAgeCategoryConfiguration());
        modelBuilder.ApplyConfiguration(new DbThemeConfiguration());
        modelBuilder.Entity<DbArticleTheme>()
       .ToTable("Themes_Article")
       .HasKey(at => at.ArticleThemeId);
        modelBuilder.ApplyConfiguration(new DbArticleReviewConfiguration());
    }
}