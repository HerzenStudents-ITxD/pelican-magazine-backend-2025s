using Microsoft.AspNetCore.Mvc;
using Backend.Contracts.Requests.User;
using Backend.Contracts.Responses.User;
using Backend.Models;
using Backend.Repositories;
using BCrypt.Net;

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
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Sec = Guid.NewGuid().ToString() // Добавлено обязательное свойство
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
}