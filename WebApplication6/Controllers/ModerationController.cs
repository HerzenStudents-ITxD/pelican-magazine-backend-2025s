using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Repositories;
using Backend.Contracts.Requests;
using Backend.Contracts.Responses;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text.Json;
using Backend.Contracts.Enums;
using System.Linq;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize(Roles = "Moderator,Admin")]
public class ModerationController : ControllerBase
{
    private readonly ArticleModerationRepository _moderationRepository;
    private readonly ArticleRepository _articleRepository;
    private readonly UserRepository _userRepository;

    public ModerationController(
        ArticleModerationRepository moderationRepository,
        ArticleRepository articleRepository,
        UserRepository userRepository)
    {
        _moderationRepository = moderationRepository;
        _articleRepository = articleRepository;
        _userRepository = userRepository;
    }

    [HttpGet("pending")]
    //[Authorize(Roles = "Moderator,Admin")]
    public async Task<IActionResult> GetPendingArticles()
    {
        var articles = await _articleRepository.GetByStatusAsync(ArticleStatus.PendingModeration);
        return Ok(articles.Select(a => new ArticleModerationResponse
        {
            ArticleId = a.ArticleId,
            ArticleTitle = a.Title,
            AuthorName = $"{a.Author.Name} {a.Author.LastName}",
            // Другие необходимые поля
        }));
    }

    [HttpGet("article/{articleId}")]
    public async Task<IActionResult> GetArticleModerationHistory(Guid articleId)
    {
        var history = await _moderationRepository.GetByArticleIdAsync(articleId);
        return Ok(history.Select(h => new ArticleModerationResponse(h)));
    }

    [HttpPost("approve/{articleId}")]
    public async Task<IActionResult> ApproveArticle(Guid articleId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return Unauthorized();

        var article = await _articleRepository.GetByIdAsync(articleId);
        if (article == null) return NotFound("Article not found");

        var moderation = new DbArticleModeration
        {
            ArticleId = articleId,
            ModeratorId = userId,
            Status = ModerationStatus.Approved,
            ModerationDate = DateTime.UtcNow
        };

        article.Status = ArticleStatus.Published;

        await _moderationRepository.AddAsync(moderation);
        await _articleRepository.UpdateAsync(article);

        return Ok(new ArticleModerationResponse(moderation));
    }

    [HttpPost("reject/{articleId}")]
    public async Task<IActionResult> RejectArticle(
        Guid articleId,
        [FromBody] RejectArticleRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return Unauthorized();

        var article = await _articleRepository.GetByIdAsync(articleId);
        if (article == null) return NotFound("Article not found");

        var moderation = new DbArticleModeration
        {
            ArticleId = articleId,
            ModeratorId = userId,
            Status = ModerationStatus.Rejected,
            Comment = request.Comment,
            RejectionReasons = JsonSerializer.Serialize(request.HighlightedParts),
            ModerationDate = DateTime.UtcNow
        };

        article.Status = ArticleStatus.Banned;

        await _moderationRepository.AddAsync(moderation);
        await _articleRepository.UpdateAsync(article);

        return Ok(new ArticleModerationResponse(moderation));
    }

    [HttpPost("request-revision/{articleId}")]
    public async Task<IActionResult> RequestRevision(
        Guid articleId,
        [FromBody] RequestRevisionRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return Unauthorized();

        var article = await _articleRepository.GetByIdAsync(articleId);
        if (article == null) return NotFound("Article not found");

        var moderation = new DbArticleModeration
        {
            ArticleId = articleId,
            ModeratorId = userId,
            Status = ModerationStatus.NeedsRevision,
            Comment = request.Comment,
            RejectionReasons = JsonSerializer.Serialize(request.HighlightedParts),
            ModerationDate = DateTime.UtcNow
        };

        article.Status = ArticleStatus.Draft;

        await _moderationRepository.AddAsync(moderation);
        await _articleRepository.UpdateAsync(article);

        return Ok(new ArticleModerationResponse(moderation));
    }
}