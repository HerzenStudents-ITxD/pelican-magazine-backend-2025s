using System.ComponentModel.DataAnnotations;

namespace Backend.Contracts.Requests;

public class CreateArticleAgeCategoryRequest
{
    [Required]
    public Guid ArticleId { get; set; }

    [Required]
    public Guid AgeCategoryId { get; set; }
}
