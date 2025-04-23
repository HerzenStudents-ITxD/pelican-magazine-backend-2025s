using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Repositories;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticleReviewsController : ControllerBase
{
    private readonly ArticleReviewRepository _articleReviewRepository;

    public ArticleReviewsController(ArticleReviewRepository articleReviewRepository)
    {
        _articleReviewRepository = articleReviewRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var reviews = await _articleReviewRepository.GetAllAsync();
        return Ok(reviews);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var review = await _articleReviewRepository.GetByIdAsync(id);
        if (review == null)
        {
            return NotFound();
        }
        return Ok(review);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DbArticleReview review)
    {
        await _articleReviewRepository.AddAsync(review);
        return CreatedAtAction(nameof(GetById), new { id = review.ReviewId }, review);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] DbArticleReview review)
    {
        if (id != review.ReviewId)
        {
            return BadRequest();
        }

        await _articleReviewRepository.UpdateAsync(review);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _articleReviewRepository.DeleteAsync(id);
        return NoContent();
    }
}