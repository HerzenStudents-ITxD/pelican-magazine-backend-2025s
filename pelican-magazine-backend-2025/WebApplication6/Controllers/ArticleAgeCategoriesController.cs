using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Repositories;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticleAgeCategoriesController : ControllerBase
{
    private readonly ArticleAgeCategoryRepository _articleAgeCategoryRepository;

    public ArticleAgeCategoriesController(ArticleAgeCategoryRepository articleAgeCategoryRepository)
    {
        _articleAgeCategoryRepository = articleAgeCategoryRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var links = await _articleAgeCategoryRepository.GetAllAsync();
        return Ok(links);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var link = await _articleAgeCategoryRepository.GetByIdAsync(id);
        if (link == null)
        {
            return NotFound();
        }
        return Ok(link);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DbArticleAgeCategory link)
    {
        await _articleAgeCategoryRepository.AddAsync(link);
        return CreatedAtAction(nameof(GetById), new { id = link.ArticleAgeId }, link);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] DbArticleAgeCategory link)
    {
        if (id != link.ArticleAgeId)
        {
            return BadRequest();
        }

        await _articleAgeCategoryRepository.UpdateAsync(link);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _articleAgeCategoryRepository.DeleteAsync(id);
        return NoContent();
    }
}