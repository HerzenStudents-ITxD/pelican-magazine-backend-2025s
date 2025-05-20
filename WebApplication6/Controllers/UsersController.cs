using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Repositories;
using Backend.Contracts.Responses;
using Backend.Contracts.Requests;
using BCrypt.Net;
using Backend.Contracts.Enums;


namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserRepository _userRepository;

    public UsersController(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponse>> GetById(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null) return NotFound();

        return Ok(new UserResponse
        {
            UserId = user.UserId,
            Name = user.Name,
            LastName = user.LastName,
            Email = user.Email
        });
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create([FromBody] CreateUserRequest request)
    {
        var user = new DbUser
        {
            Name = request.Name,
            LastName = request.LastName,
            Email = request.Email,
            Birth = request.Birth,
            Sex = request.Sex,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, BCrypt.Net.BCrypt.GenerateSalt()),
            Sec = Guid.NewGuid().ToString()
        };

        await _userRepository.AddAsync(user);

        return CreatedAtAction(nameof(GetById), new { id = user.UserId }, new UserResponse
        {
            UserId = user.UserId,
            Name = user.Name,
            LastName = user.LastName,
            Email = user.Email
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null) return NotFound();

        user.Name = request.Name;
        user.LastName = request.LastName;
        user.Nickname = request.Nickname;

        await _userRepository.UpdateAsync(user);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _userRepository.DeleteAsync(id);
        return NoContent();
    }
}