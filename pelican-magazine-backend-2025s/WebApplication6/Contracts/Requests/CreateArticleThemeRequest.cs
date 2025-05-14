using System.ComponentModel.DataAnnotations;

namespace Backend.Contracts.Requests;

public class CreateArticleThemeRequest
{
    [Required]
    public Guid ArticleId { get; set; }

    [Required]
    public Guid ThemeId { get; set; }
}