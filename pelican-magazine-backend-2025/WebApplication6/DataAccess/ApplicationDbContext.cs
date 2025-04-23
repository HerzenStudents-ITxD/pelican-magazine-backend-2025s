using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.DataAccess;

public class ApplicationDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public ApplicationDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public DbSet<DbUser> Users { get; set; }
    public DbSet<DbArticle> Articles { get; set; }
    public DbSet<DbAgeCategory> AgeCategories { get; set; }
    public DbSet<DbArticleAgeCategory> ArticleAgeCategories { get; set; }
    public DbSet<DbTheme> Themes { get; set; }
    public DbSet<DbArticleTheme> ArticleThemes { get; set; }
    public DbSet<DbTitle> Titles { get; set; }
    public DbSet<DbArticleReview> ArticleReviews { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_configuration.GetConnectionString("Database"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new DbUserConfiguration());
        modelBuilder.ApplyConfiguration(new DbArticleConfiguration());
        modelBuilder.ApplyConfiguration(new DbAgeCategoryConfiguration());
        modelBuilder.ApplyConfiguration(new DbArticleAgeCategoryConfiguration());
        modelBuilder.ApplyConfiguration(new DbThemeConfiguration());
        modelBuilder.ApplyConfiguration(new DbArticleThemeConfiguration());
        modelBuilder.ApplyConfiguration(new DbTitleConfiguration());
        modelBuilder.ApplyConfiguration(new DbArticleReviewConfiguration());
    }
}