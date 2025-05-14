using System.ComponentModel.DataAnnotations;

namespace Backend.Contracts.Requests;

public class UpdateArticleRequest
{
    [Required][StringLength(100)] public string Title { get; set; } = string.Empty;
    [Required][StringLength(500)] public string Description { get; set; } = string.Empty;
    [Required] public string Text { get; set; } = string.Empty;
    public string? Thumbnail { get; set; }
}