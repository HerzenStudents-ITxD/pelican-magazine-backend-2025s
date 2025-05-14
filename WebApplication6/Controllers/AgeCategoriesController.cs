using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Repositories;
using Backend.Contracts.Requests;
using Backend.Contracts.Responses;
using Backend.Contracts.Enums;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AgeCategoriesController : ControllerBase
{
    private readonly AgeCategoryRepository _ageCategoryRepository;

    public AgeCategoriesController(AgeCategoryRepository ageCategoryRepository)
    {
        _ageCategoryRepository = ageCategoryRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _ageCategoryRepository.GetAllAsync();
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var category = await _ageCategoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }
        return Ok(category);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAgeCategoryRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); // Вернёт ошибку, если CategoryName пустое
        }

        var category = new DbAgeCategory
        {
            CategoryName = request.CategoryName
        };

        await _ageCategoryRepository.AddAsync(category);
        return CreatedAtAction(nameof(GetById), new { id = category.AgeCategoryId }, category);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] DbAgeCategory category)
    {
        if (id != category.AgeCategoryId)
        {
            return BadRequest();
        }

        await _ageCategoryRepository.UpdateAsync(category);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _ageCategoryRepository.DeleteAsync(id);
        return NoContent();
    }
}