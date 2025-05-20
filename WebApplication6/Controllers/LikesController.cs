
using Backend.Models;
using Backend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize]
public class LikesController : ControllerBase
{
    private readonly LikeRepository _likeRepository;
    private readonly ArticleRepository _articleRepository;

    public LikesController(
        LikeRepository likeRepository,
        ArticleRepository articleRepository)
    {
        _likeRepository = likeRepository;
        _articleRepository = articleRepository;
    }

    [HttpPost("{articleId}")]
    public async Task<IActionResult> ToggleLike(Guid articleId)
    {
        //var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
       // var userId = Guid.Parse("a1b2c3d4-1234-5678-9101-111213141516"); // Test
        var article = await _articleRepository.GetByIdAsync(articleId);
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("User ID not found in token");
        }

        var existingLike = await _likeRepository.GetByUserAndArticleAsync(userId, articleId);

        if (existingLike != null)
        {
            await _likeRepository.DeleteAsync(existingLike.LikeId);
            return Ok(new { Liked = false });
        }
        else
        {
            var newLike = new DbLike
            {
                UserId = userId,
                ArticleId = articleId
            };
            await _likeRepository.AddAsync(newLike);
            return Ok(new { Liked = true });
        }
    }

    [HttpGet("article/{articleId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLikesForArticle(Guid articleId)
    {
        var count = await _likeRepository.GetCountForArticleAsync(articleId);
        return Ok(new { Count = count });
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetUserLikes()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var likes = await _likeRepository.GetByUserIdAsync(userId);
        return Ok(likes);
    }
}