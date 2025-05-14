using Backend.Models;

namespace Backend.Contracts.Responses;

public class ReviewDetailResponse : ReviewResponse
{
    public string ArticleTitle { get; set; } = string.Empty;

    public ReviewDetailResponse(DbArticleReview review) : base(review)
    {
        ArticleTitle = review.Article.Title;
    }
}