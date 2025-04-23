using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

public class DbAgeCategory
{
    public const string TableName = "AgeCategories";

    public int AgeCategoryId { get; set; }
    public string CategoryName { get; set; }

    [NotMapped]
    public ICollection<DbArticleAgeCategory> ArticleAgeCategories { get; set; }
}

public class DbAgeCategoryConfiguration : IEntityTypeConfiguration<DbAgeCategory>
{
    public void Configure(EntityTypeBuilder<DbAgeCategory> builder)
    {
        builder.ToTable(DbAgeCategory.TableName);
        builder.HasKey(ac => ac.AgeCategoryId);
    }
}