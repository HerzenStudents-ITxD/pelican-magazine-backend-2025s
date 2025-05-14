using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

public class DbTheme
{
    public const string TableName = "Themes";

    public Guid ThemeId { get; set; } = Guid.NewGuid();
    public string ThemeName { get; set; }

    public ICollection<DbArticleTheme> ArticleThemes { get; set; }
}

public class DbThemeConfiguration : IEntityTypeConfiguration<DbTheme>
{
    public void Configure(EntityTypeBuilder<DbTheme> builder)
    {
        builder.ToTable(DbTheme.TableName);
        builder.HasKey(t => t.ThemeId);
        builder.Property(t => t.ThemeName).IsRequired().HasMaxLength(100);
    }
}