using Backend.Contracts.Responses.Article;

namespace Backend.Contracts.Responses.Article;

public class ArticleDetailResponse 
{
    public string Text { get; set; }
    public DateTime ChangedAt { get; set; }
    public string Status { get; set; }
}