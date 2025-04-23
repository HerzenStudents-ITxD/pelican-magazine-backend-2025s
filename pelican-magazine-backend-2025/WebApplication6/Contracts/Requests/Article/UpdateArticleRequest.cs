using System.ComponentModel.DataAnnotations;

namespace Backend.Contracts.Requests.Article;

public class UpdateArticleRequest
{
    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    [Required]
    [StringLength(500)]
    public string Description { get; set; }

    [Required]
    public string Text { get; set; }

    public string? Thumbnail { get; set; }
}