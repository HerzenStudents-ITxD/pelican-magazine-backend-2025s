using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Repositories;
using Backend.Contracts.Requests;
using Backend.Contracts.Responses;
using System.ComponentModel.DataAnnotations;
using Backend.Contracts.Enums;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly ArticleRepository _articleRepository;
    private readonly UserRepository _userRepository;
    private readonly ArticleAgeCategoryRepository _ageCategoryRepository;
    private readonly ArticleThemeRepository _themeRepository;

    public ArticlesController(
        ArticleRepository articleRepository,
        UserRepository userRepository,
        ArticleAgeCategoryRepository ageCategoryRepository,
        ArticleThemeRepository themeRepository)
    {
        _articleRepository = articleRepository;
        _userRepository = userRepository;
        _ageCategoryRepository = ageCategoryRepository;
        _themeRepository = themeRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ArticleFilterRequest filter)
    {
        var articles = (await _articleRepository.GetAllAsync()).AsQueryable();

        // Фильтрация
        if (!string.IsNullOrEmpty(filter.Search))
        {
            articles = articles.Where(a =>
                a.Title.Contains(filter.Search, StringComparison.OrdinalIgnoreCase) ||
                a.Description.Contains(filter.Search, StringComparison.OrdinalIgnoreCase));
        }

        // Сортировка
        var sortedArticles = filter.SortOrder?.ToLower() == "desc"
            ? articles.OrderByDescending(a => a.ChangedAt).ToList()
            : articles.OrderBy(a => a.ChangedAt).ToList();

        var response = sortedArticles.Select(a => new ArticleResponse(a));
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var article = await _articleRepository.GetByIdAsync(id);
        if (article == null)
        {
            return NotFound();
        }

        var ageCategories = (await _ageCategoryRepository.GetByArticleIdAsync(id))
            .Select(ac => new AgeCategoryResponse(ac.AgeCategory))
            .ToList();

        var themes = (await _themeRepository.GetByArticleIdAsync(id))
            .Select(t => new ThemeResponse(t.Theme))
            .ToList();

        var response = new ArticleDetailResponse(article)
        {
            AgeCategories = ageCategories,
            Themes = themes
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateArticleRequest request)
    {
        // Валидация
        var author = await _userRepository.GetByIdAsync(request.AuthorId);
        if (author == null)
        {
            return BadRequest("Author not found");
        }

        // Создание статьи
        var article = new DbArticle
        {
            Title = request.Title,
            Description = request.Description,
            Text = request.Text,
            Thumbnail = request.Thumbnail,
            AuthorId = request.AuthorId,
            Author = author,
            Status = ArticleStatus.Draft,
            ChangedAt = DateTime.UtcNow
        };

        await _articleRepository.AddAsync(article);

        // Добавление возрастных категорий
        foreach (var ageCategoryId in request.AgeCategoryIds)
        {
            await _ageCategoryRepository.AddAsync(new DbArticleAgeCategory
            {
                ArticleId = article.ArticleId,
                AgeCategoryId = ageCategoryId
            });
        }

        // Добавление тем
        foreach (var themeId in request.ThemeIds)
        {
            await _themeRepository.AddAsync(new DbArticleTheme
            {
                ArticleId = article.ArticleId,
                ThemeId = themeId
            });
        }

        return CreatedAtAction(nameof(GetById), new { id = article.ArticleId }, new ArticleResponse(article));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateArticleRequest request)
    {
        var article = await _articleRepository.GetByIdAsync(id);
        if (article == null)
        {
            return NotFound();
        }

        article.Title = request.Title;
        article.Description = request.Description;
        article.Text = request.Text;
        article.Thumbnail = request.Thumbnail;
        article.ChangedAt = DateTime.UtcNow;

        await _articleRepository.UpdateAsync(article);
        return NoContent();
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateArticleStatusRequest request)
    {
        var article = await _articleRepository.GetByIdAsync(id);
        if (article == null)
        {
            return NotFound();
        }

        article.Status = (Backend.Contracts.Enums.ArticleStatus)request.Status;
        article.ChangedAt = DateTime.UtcNow;

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