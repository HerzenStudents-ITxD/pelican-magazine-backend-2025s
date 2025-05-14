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
    public Guid? ThemeId { get; set; }
    public int? Year { get; set; } // Год поступления/окончания
    public int? Course { get; set; } // Курс (для студентов)
    public string? Degree { get; set; } // Степень/квалификация
    public required string Email { get; set; }
    public required string Sex { get; set; }
    public required string Sec { get; set; } = Guid.NewGuid().ToString();
    public DateTime Birth { get; set; }
    public string? ProfileImg { get; set; }
    public string? ProfileCover { get; set; }
    // Флаг администратора (лучше через роль, но можно и так)
    public bool IsAdmin { get; set; } = false;
    public DbTheme? Theme { get; set; }
    public string? TwoFactorSecret { get; set; } // Секрет для 2FA
    public bool TwoFactorEnabled { get; set; } = false; // Включена ли 2FA
    public string? EmailVerificationToken { get; set; } // Токен верификации email
    public bool EmailVerified { get; set; } = false; // Подтвержден ли email
    public DateTime? EmailVerificationTokenExpiry { get; set; } // Срок действия токена
                                                                // Роль пользователя
    public UserType UserType { get; set; } = UserType.Other;


    // Навигационные свойства
    public ICollection<DbArticle> Articles { get; set; } = new List<DbArticle>();
    public ICollection<DbLike> Likes { get; set; } = new List<DbLike>();
    public ICollection<DbArticleReview> Reviews { get; set; } = new List<DbArticleReview>();

    public ICollection<DbArticleReview> ArticleReviews { get; set; } = new List<DbArticleReview>();

   




    public class DbUserConfiguration : IEntityTypeConfiguration<DbUser>
    {
        public void Configure(EntityTypeBuilder<DbUser> builder)
        {
            builder.ToTable(DbUser.TableName);
            builder.HasKey(u => u.UserId);

            builder.Property(u => u.Name).IsRequired().HasMaxLength(100);
            builder.Property(u => u.LastName).IsRequired().HasMaxLength(100);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(255);
            builder.Property(u => u.PasswordHash).IsRequired();
            builder.Property(u => u.Sex).IsRequired().HasDefaultValue("unknown");
            builder.Property(u => u.Sec).IsRequired();

            builder.HasMany(u => u.Articles)
                .WithOne(a => a.Author)
                .HasForeignKey(a => a.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.ArticleReviews)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}