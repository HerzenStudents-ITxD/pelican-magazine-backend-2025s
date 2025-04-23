using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Repositories;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly ArticleRepository _articleRepository;

    public ArticlesController(ArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var articles = await _articleRepository.GetAllAsync();
        return Ok(articles);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var article = await _articleRepository.GetByIdAsync(id);
        if (article == null)
        {
            return NotFound();
        }
        return Ok(article);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DbArticle article)
    {
        await _articleRepository.AddAsync(article);
        return CreatedAtAction(nameof(GetById), new { id = article.ArticleId }, article);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] DbArticle article)
    {
        if (id != article.ArticleId)
        {
            return BadRequest();
        }

        await _articleRepository.UpdateAsync(article);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _articleRepository.DeleteAsync(id);
        return NoContent();
    }
}