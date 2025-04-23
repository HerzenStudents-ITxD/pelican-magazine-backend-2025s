using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Repositories;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TitlesController : ControllerBase
{
    private readonly TitleRepository _titleRepository;

    public TitlesController(TitleRepository titleRepository)
    {
        _titleRepository = titleRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var titles = await _titleRepository.GetAllAsync();
        return Ok(titles);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var title = await _titleRepository.GetByIdAsync(id);
        if (title == null)
        {
            return NotFound();
        }
        return Ok(title);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DbTitle title)
    {
        await _titleRepository.AddAsync(title);
        return CreatedAtAction(nameof(GetById), new { id = title.TitleId }, title);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] DbTitle title)
    {
        if (id != title.TitleId)
        {
            return BadRequest();
        }

        await _titleRepository.UpdateAsync(title);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _titleRepository.DeleteAsync(id);
        return NoContent();
    }
}