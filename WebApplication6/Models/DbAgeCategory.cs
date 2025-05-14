using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

public class DbAgeCategory
{
    public const string TableName = "AgeCategories";

    public Guid AgeCategoryId { get; set; }
    public string CategoryName { get; set; }


    public ICollection<DbArticleAgeCategory> ArticleAgeCategories { get; set; } = new List<DbArticleAgeCategory>();
}

public class DbAgeCategoryConfiguration : IEntityTypeConfiguration<DbAgeCategory>
{
    public void Configure(EntityTypeBuilder<DbAgeCategory> builder)
    {
        builder.Property(ac => ac.AgeCategoryId)
    .HasColumnType("uniqueidentifier") // Для SQL Server
    .IsRequired();
        builder.ToTable(DbAgeCategory.TableName);
        builder.HasKey(ac => ac.AgeCategoryId);
    }
}