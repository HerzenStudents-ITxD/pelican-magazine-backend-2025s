using Backend.Models;

namespace Backend.Contracts.Responses;

public class ArticleDetailResponse : ArticleResponse
{
    public string Text { get; set; } = string.Empty;
    public List<AgeCategoryResponse> AgeCategories { get; set; } = new();
    public List<ThemeResponse> Themes { get; set; } = new();

    public ArticleDetailResponse(DbArticle article) : base(article)
    {
        Text = article.Text;
    }
}