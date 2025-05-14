using Backend.Models;

namespace Backend.Contracts.Responses;

public class ArticleResponse
{
    public Guid ArticleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Thumbnail { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
    public string Status { get; set; } = string.Empty;

    public ArticleResponse(DbArticle article)
    {
        ArticleId = article.ArticleId;
        Title = article.Title;
        Description = article.Description;
        Thumbnail = article.Thumbnail;
        AuthorName = $"{article.Author?.Name} {article.Author?.LastName}";
        ChangedAt = article.ChangedAt;
        Status = article.Status.ToString();
    }
}