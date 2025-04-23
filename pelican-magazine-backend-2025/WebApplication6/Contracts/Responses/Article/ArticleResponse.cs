using Backend.Models;

namespace Backend.Contracts.Responses.Article;

public class ArticleResponse
{
    public Guid ArticleId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string? Thumbnail { get; set; }

    public ArticleResponse(DbArticle article)
    {
        ArticleId = article.ArticleId;
        Title = article.Title;
        Description = article.Description;
        Thumbnail = article.Thumbnail;
    }
}