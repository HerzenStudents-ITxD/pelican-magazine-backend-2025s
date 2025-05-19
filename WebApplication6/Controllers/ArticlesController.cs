using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Repositories;
using Backend.Contracts.Requests;
using Backend.Contracts.Responses;
using System.ComponentModel.DataAnnotations;
using Backend.Contracts.Enums;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly ArticleRepository _articleRepository;
    private readonly UserRepository _userRepository;
    private readonly ArticleAgeCategoryRepository _ageCategoryRepository;
    private readonly ArticleThemeRepository _themeRepository;
    private readonly LikeRepository _likeRepository;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ArticlesController(
        ArticleRepository articleRepository,
        UserRepository userRepository,
        ArticleAgeCategoryRepository ageCategoryRepository,
        ArticleThemeRepository themeRepository,
        LikeRepository likeRepository,
        IWebHostEnvironment webHostEnvironment)
    {
        _articleRepository = articleRepository;
        _userRepository = userRepository;
        _ageCategoryRepository = ageCategoryRepository;
        _themeRepository = themeRepository;
        _likeRepository = likeRepository;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByAuthorId(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return NotFound("User not found");
        }

        var articles = await _articleRepository.GetByAuthorIdAsync(userId);
        var response = articles.Select(a => new ArticleResponse(a));
        return Ok(response);
    }

    [HttpGet("liked/{userId}")]
    [Authorize]
    public async Task<IActionResult> GetLikedArticles(Guid userId)
    {
        // Проверка, что запрашивающий пользователь имеет доступ
        var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (currentUserId != userId)
        {
            return Forbid();
        }

        var likes = await _likeRepository.GetByUserIdAsync(userId);
        var articleIds = likes.Select(l => l.ArticleId).Distinct().ToList();

        var articles = new List<DbArticle>();
        foreach (var articleId in articleIds)
        {
            var article = await _articleRepository.GetByIdAsync(articleId);
            if (article != null)
            {
                articles.Add(article);
            }
        }

        return Ok(articles.Select(a => new ArticleResponse(a)));
    }

    [HttpPost("{id}/cover")]
    [Authorize]
    public async Task<IActionResult> UploadCover(Guid id, IFormFile file)
    {
        try
        {
            // 1. Проверка наличия файла
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            // 2. Проверка типа файла
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                return BadRequest("Invalid file type. Only JPG, JPEG, PNG and WEBP are allowed.");

            // 3. Проверка размера файла (10MB)
            if (file.Length > 10 * 1024 * 1024)
                return BadRequest("File size exceeds limit (10MB)");

            // 4. Получение статьи
            var article = await _articleRepository.GetByIdAsync(id);
            if (article == null)
                return NotFound("Article not found");

            // 5. Проверка прав пользователя
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (article.AuthorId != userId)
                return Forbid();

            // 6. Генерация уникального имени файла
            var fileName = $"{Guid.NewGuid()}{extension}";
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "article-covers");
            var filePath = Path.Combine(uploadsFolder, fileName);

            // 7. Создание папки, если не существует
            Directory.CreateDirectory(uploadsFolder);

            // 8. Сохранение файла
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 9. Обновление обложки в БД
            var coverUrl = $"/article-covers/{fileName}";
            article.Thumbnail = coverUrl;
            await _articleRepository.UpdateAsync(article);

            return Ok(new { CoverUrl = coverUrl });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
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
            .Select(ac => new AgeCategoryResponse(ac.AgeCategory)) // Используем связанный объект AgeCategory
            .ToList();

        var themes = (await _themeRepository.GetByArticleIdAsync(id))
            .Select(t => new ThemeResponse(t.Theme)) // Используем связанный объект Theme
            .ToList();

        var response = new ArticleDetailResponse(article)
        {
            AgeCategories = ageCategories,
            Themes = themes
        };

        return Ok(response);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateArticle([FromBody] CreateArticleRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null) return Unauthorized();

        var article = new DbArticle
        {
            Title = request.Title,
            Description = request.Description,
            Text = request.Text,
            Summary = request.Summary,
            Thumbnail = request.Thumbnail,
            AuthorId = userId,
            Author = user,
            Status = ArticleStatus.PendingModeration,
            ChangedAt = DateTime.UtcNow
        };

        await _articleRepository.AddAsync(article);

        // Обработка возрастного ограничения
        if (!string.IsNullOrEmpty(request.AgeRestriction))
        {
            var ageCategories = await _ageCategoryRepository.GetAllAsync();
            var ageCategory = ageCategories.FirstOrDefault(ac =>
                ac.CategoryName.Equals(request.AgeRestriction, StringComparison.OrdinalIgnoreCase));

            if (ageCategory != null)
            {
                await _ageCategoryRepository.AddAsync(new DbArticleAgeCategory
                {
                    ArticleId = article.ArticleId,
                    AgeCategoryId = ageCategory.AgeCategoryId
                });
            }
        }

        // Обработка категорий
        foreach (var categoryName in request.Categories)
        {
            var themes = await _themeRepository.GetAllAsync();
            var theme = themes.FirstOrDefault(t =>
                t.ThemeName.Equals(categoryName, StringComparison.OrdinalIgnoreCase));

            if (theme != null)
            {
                await _themeRepository.AddAsync(new DbArticleTheme
                {
                    ArticleId = article.ArticleId,
                    ThemeId = theme.ThemeId
                });
            }
        }

        return CreatedAtAction(nameof(GetById), new { id = article.ArticleId },
            new ArticleResponse(article));
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