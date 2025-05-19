using System.ComponentModel.DataAnnotations;

namespace Backend.Contracts.Requests;

public class CreateArticleRequest
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Text { get; set; } = string.Empty;

    [Required]
    public string Summary { get; set; } = string.Empty;

    public List<string> Categories { get; set; } = new();
    public string AgeRestriction { get; set; } = "16+";
    public string? Thumbnail { get; set; }
}