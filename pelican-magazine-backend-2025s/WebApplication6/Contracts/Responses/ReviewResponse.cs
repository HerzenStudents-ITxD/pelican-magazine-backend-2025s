using Backend.Models;

namespace Backend.Contracts.Responses;

public class ReviewResponse
{
    public Guid ReviewId { get; set; }
    public Guid ArticleId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime ReviewDate { get; set; }
    public string Comments { get; set; } = string.Empty;

    public ReviewResponse(DbArticleReview review)
    {
        ReviewId = review.ReviewId;
        ArticleId = review.ArticleId;
        UserId = review.UserId;
        UserName = $"{review.User.Name} {review.User.LastName}";
        ReviewDate = review.ReviewDate;
        Comments = review.Comments;
    }
}