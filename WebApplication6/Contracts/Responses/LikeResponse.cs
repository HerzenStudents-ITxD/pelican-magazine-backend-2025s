
namespace Backend.Contracts.Responses;

public class LikeResponse
{
    public Guid LikeId { get; set; }
    public Guid ArticleId { get; set; }
    public Guid UserId { get; set; }
    public DateTime LikedAt { get; set; }
}