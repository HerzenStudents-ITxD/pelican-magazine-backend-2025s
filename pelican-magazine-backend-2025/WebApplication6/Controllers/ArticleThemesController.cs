using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Repositories;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticleThemesController : ControllerBase
{
    private readonly ArticleThemeRepository _articleThemeRepository;

    public ArticleThemesController(ArticleThemeRepository articleThemeRepository)
    {
        _articleThemeRepository = articleThemeRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var links = await _articleThemeRepository.GetAllAsync();
        return Ok(links);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var link = await _articleThemeRepository.GetByIdAsync(id);
        if (link == null)
        {
            return NotFound();
        }
        return Ok(link);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DbArticleTheme link)
    {
        await _articleThemeRepository.AddAsync(link);
        return CreatedAtAction(nameof(GetById), new { id = link.ArticleThemeId }, link);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] DbArticleTheme link)
    {
        if (id != link.ArticleThemeId)
        {
            return BadRequest();
        }

        await _articleThemeRepository.UpdateAsync(link);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _articleThemeRepository.DeleteAsync(id);
        return NoContent();
    }
}