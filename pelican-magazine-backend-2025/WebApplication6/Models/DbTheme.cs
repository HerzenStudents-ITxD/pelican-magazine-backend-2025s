using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;
public class DbTheme
{
    public const string TableName = "Themes";

    public int ThemeId { get; set; }
    public string ThemeName { get; set; }

    [NotMapped]
    public ICollection<DbArticleTheme> ArticleThemes { get; set; }
}

public class DbThemeConfiguration : IEntityTypeConfiguration<DbTheme>
{
    public void Configure(EntityTypeBuilder<DbTheme> builder)
    {
        builder.ToTable(DbTheme.TableName);
        builder.HasKey(t => t.ThemeId);
    }
}