using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Repositories;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ThemesController : ControllerBase
{
    private readonly ThemeRepository _themeRepository;

    public ThemesController(ThemeRepository themeRepository)
    {
        _themeRepository = themeRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var themes = await _themeRepository.GetAllAsync();
        return Ok(themes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var theme = await _themeRepository.GetByIdAsync(id);
        if (theme == null)
        {
            return NotFound();
        }
        return Ok(theme);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DbTheme theme)
    {
        await _themeRepository.AddAsync(theme);
        return CreatedAtAction(nameof(GetById), new { id = theme.ThemeId }, theme);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] DbTheme theme)
    {
        if (id != theme.ThemeId)
        {
            return BadRequest();
        }

        await _themeRepository.UpdateAsync(theme);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _themeRepository.DeleteAsync(id);
        return NoContent();
    }
}