using Backend.Contracts.Enums;

namespace Backend.Contracts.Requests;

public class UpdateArticleStatusRequest
{
    public ArticleStatus Status { get; set; }
}