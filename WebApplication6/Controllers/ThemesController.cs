using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Repositories;
using Backend.Contracts.Requests;
using Backend.Contracts.Responses;
using System.ComponentModel.DataAnnotations;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ThemesController : ControllerBase
{
    private readonly ThemeRepository _repository;

    public ThemesController(ThemeRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var themes = await _repository.GetAllAsync();
        var response = themes.Select(t => new ThemeResponse(t));
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var theme = await _repository.GetByIdAsync(id);
        if (theme == null)
        {
            return NotFound();
        }
        return Ok(new ThemeResponse(theme));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateThemeRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var theme = new DbTheme
        {
            ThemeName = request.ThemeName
        };

        await _repository.AddAsync(theme);
        return CreatedAtAction(nameof(GetById), new { id = theme.ThemeId }, new ThemeResponse(theme));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateThemeRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var theme = await _repository.GetByIdAsync(id);
        if (theme == null)
        {
            return NotFound();
        }

        theme.ThemeName = request.ThemeName;
        await _repository.UpdateAsync(theme);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var theme = await _repository.GetByIdAsync(id);
        if (theme == null)
        {
            return NotFound();
        }

        await _repository.DeleteAsync(id);
        return NoContent();
    }
}