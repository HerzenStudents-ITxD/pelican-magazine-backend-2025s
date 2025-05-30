﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

public class DbArticleTheme
{
    public const string TableName = "Article_Themes";

    public Guid ArticleThemeId { get; set; } = Guid.NewGuid();
    public Guid ThemeId { get; set; }
    public Guid ArticleId { get; set; }

    [ForeignKey("ThemeId")]
    public DbTheme Theme { get; set; }

    public string ThemeName { get; set; }

    [ForeignKey("ArticleId")]
    public DbArticle Article { get; set; }
}

public class DbArticleThemeConfiguration : IEntityTypeConfiguration<DbArticleTheme>
{
    public void Configure(EntityTypeBuilder<DbArticleTheme> builder)
    {
        builder.ToTable(DbArticleTheme.TableName);
        builder.HasKey(at => at.ArticleThemeId);

        builder.HasOne(at => at.Theme)
            .WithMany(t => t.ArticleThemes)
            .HasForeignKey(at => at.ThemeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(at => at.Article)
            .WithMany(a => a.ArticleThemes)
            .HasForeignKey(at => at.ArticleId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}