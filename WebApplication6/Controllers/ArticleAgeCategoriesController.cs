using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Repositories;
using Backend.Contracts.Requests;
using Backend.Contracts.Responses;
using Backend.Contracts.Enums;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticleAgeCategoriesController : ControllerBase
{
    private readonly ArticleAgeCategoryRepository _repository;
    private readonly ArticleRepository _articleRepository;
    private readonly AgeCategoryRepository _ageCategoryRepository;

    public ArticleAgeCategoriesController(
        ArticleAgeCategoryRepository repository,
        ArticleRepository articleRepository,
        AgeCategoryRepository ageCategoryRepository)
    {
        _repository = repository;
        _articleRepository = articleRepository;
        _ageCategoryRepository = ageCategoryRepository;
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
    [FromBody] CreateArticleAgeCategoryRequest request)
    {
        // Проверка существования сущностей
        var article = await _articleRepository.GetByIdAsync(request.ArticleId);
        var category = await _ageCategoryRepository.GetByIdAsync(request.AgeCategoryId);

        if (article == null || category == null)
        {
            return BadRequest(
                article == null
                    ? "Article not found"
                    : "AgeCategory not found");
        }

        var link = new DbArticleAgeCategory
        {
            ArticleId = request.ArticleId,
            AgeCategoryId = request.AgeCategoryId
        };

        await _repository.AddAsync(link);
        return Ok();
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