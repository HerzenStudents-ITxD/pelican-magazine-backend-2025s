using Backend.Contracts.Requests;
using Backend.Models;
using System.Text.Json;

namespace Backend.Contracts.Responses;

public class ArticleModerationResponse
{
    public Guid ModerationId { get; set; }
    public Guid ArticleId { get; set; }
    public string ArticleTitle { get; set; } = string.Empty;
    public Guid ModeratorId { get; set; }
    public string ModeratorName { get; set; } = string.Empty;
    public DateTime ModerationDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public List<HighlightedPart>? HighlightedParts { get; set; }

    public ArticleModerationResponse(DbArticleModeration moderation)
    {
        ModerationId = moderation.ModerationId;
        ArticleId = moderation.ArticleId;
        ArticleTitle = moderation.Article?.Title ?? string.Empty;
        ModeratorId = moderation.ModeratorId;
        ModeratorName = $"{moderation.Moderator?.Name} {moderation.Moderator?.LastName}";
        ModerationDate = moderation.ModerationDate;
        Status = moderation.Status.ToString();
        Comment = moderation.Comment;

        if (!string.IsNullOrEmpty(moderation.RejectionReasons))
        {
            HighlightedParts = JsonSerializer.Deserialize<List<HighlightedPart>>(moderation.RejectionReasons);
        }
    }
}