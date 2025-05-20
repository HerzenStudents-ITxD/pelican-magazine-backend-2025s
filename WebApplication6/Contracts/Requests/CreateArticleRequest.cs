using System.ComponentModel.DataAnnotations;

public class CreateArticleRequest
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; }

    [Required]
    [StringLength(500)]
    public string Description { get; set; }

    [Required]
    public string Text { get; set; }

    public string Summary { get; set; }

    
    [RegularExpression(@"^[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}$",
        ErrorMessage = "Invalid GUID format")]
    public string AuthorId { get; set; }

    public List<string> Categories { get; set; } = new();

    public string AgeRestriction { get; set; } = "16+";

    public string Thumbnail { get; set; }
}