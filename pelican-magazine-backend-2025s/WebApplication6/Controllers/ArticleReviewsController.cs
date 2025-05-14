using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Repositories;
using Backend.Contracts.Requests;
using Backend.Contracts.Responses;
using Backend.Contracts.Enums;


namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticleReviewsController : ControllerBase
{
    private readonly ArticleReviewRepository _repository;
    private readonly ArticleRepository _articleRepository;
    private readonly UserRepository _userRepository;

    public ArticleReviewsController(
        ArticleReviewRepository repository,
        ArticleRepository articleRepository,
        UserRepository userRepository)
    {
        _repository = repository;
        _articleRepository = articleRepository;
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var reviews = await _repository.GetAllAsync();
        return Ok(reviews.Select(r => new ReviewResponse(r)));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var review = await _repository.GetByIdAsync(id);
        if (review == null) return NotFound();
        return Ok(new ReviewDetailResponse(review));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReviewRequest request)
    {
        if (await _articleRepository.GetByIdAsync(request.ArticleId) == null ||
            await _userRepository.GetByIdAsync(request.UserId) == null)
        {
            return BadRequest("Article or User not found");
        }

        var review = new DbArticleReview
        {
            ArticleId = request.ArticleId,
            UserId = request.UserId,
            Comments = request.Comments
        };

        await _repository.AddAsync(review);
        return CreatedAtAction(nameof(GetById), new { id = review.ReviewId }, new ReviewResponse(review));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("by-article/{articleId}")]
    public async Task<IActionResult> GetByArticleId(Guid articleId)
    {
        var reviews = await _repository.GetByArticleIdAsync(articleId);
        return Ok(reviews.Select(r => new ReviewResponse(r)));
    }
}