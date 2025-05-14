using System.ComponentModel.DataAnnotations;

namespace Backend.Contracts.Requests;

public class CreateReviewRequest
{
    [Required]
    public Guid ArticleId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [StringLength(1000)]
    public string Comments { get; set; } = string.Empty;
}