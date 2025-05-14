using System.ComponentModel.DataAnnotations;

namespace Backend.Contracts.Requests;

public class CreateArticleRequest
{
    [Required][StringLength(100)] public string Title { get; set; } = string.Empty;
    [Required][StringLength(500)] public string Description { get; set; } = string.Empty;
    [Required] public string Text { get; set; } = string.Empty;
    public string? Thumbnail { get; set; }
    [Required] public Guid AuthorId { get; set; }
    public List<Guid> AgeCategoryIds { get; set; } = new();
    public List<Guid> ThemeIds { get; set; } = new();
}