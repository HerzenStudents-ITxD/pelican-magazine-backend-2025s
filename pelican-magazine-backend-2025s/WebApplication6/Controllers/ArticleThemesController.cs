using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Repositories;
using Backend.Contracts.Requests;
using Backend.Contracts.Responses;
using Backend.Contracts.Enums;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticleThemesController : ControllerBase
{
    private readonly ArticleThemeRepository _repository;
    private readonly ArticleRepository _articleRepo;
    private readonly ThemeRepository _themeRepo;

    public ArticleThemesController(
        ArticleThemeRepository repository,
        ArticleRepository articleRepo,
        ThemeRepository themeRepo)
    {
        _repository = repository;
        _articleRepo = articleRepo;
        _themeRepo = themeRepo;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _repository.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateArticleThemeRequest request)
    {
        // Проверка существования статьи и темы
        var article = await _articleRepo.GetByIdAsync(request.ArticleId);
        var theme = await _themeRepo.GetByIdAsync(request.ThemeId);

        if (article == null || theme == null)
        {
            return BadRequest(article == null ? "Article not found" : "Theme not found");
        }

        var link = new DbArticleTheme
        {
            ArticleId = request.ArticleId,
            ThemeId = request.ThemeId
        };

        await _repository.AddAsync(link);
        return CreatedAtAction(nameof(GetById), new { id = link.ArticleThemeId }, link);
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
        var items = await _repository.GetByArticleIdAsync(articleId);
        return Ok(items);
    }
}