using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Repositories;

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
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _ageCategoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }
        return Ok(category);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DbAgeCategory category)
    {
        await _ageCategoryRepository.AddAsync(category);
        return CreatedAtAction(nameof(GetById), new { id = category.AgeCategoryId }, category);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] DbAgeCategory category)
    {
        if (id != category.AgeCategoryId)
        {
            return BadRequest();
        }

        await _ageCategoryRepository.UpdateAsync(category);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _ageCategoryRepository.DeleteAsync(id);
        return NoContent();
    }
}