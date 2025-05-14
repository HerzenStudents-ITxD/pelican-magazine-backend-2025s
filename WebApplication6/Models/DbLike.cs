// Models/DbLike.cs
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

public class DbLike
{
    public Guid LikeId { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid ArticleId { get; set; }
    public DateTime LikedAt { get; set; } = DateTime.UtcNow;

    // Навигационные свойства
    [ForeignKey("UserId")]
    public DbUser User { get; set; }

    [ForeignKey("ArticleId")]
    public DbArticle Article { get; set; }
}