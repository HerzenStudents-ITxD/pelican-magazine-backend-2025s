using Backend.Contracts.Requests;
using Backend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IO;

namespace Backend.Controllers;

[ApiController]
[Route("api/user-profile")]

public class UserProfileController : ControllerBase
{
    private readonly UserRepository _userRepository;

    public UserProfileController(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile(UpdateUserRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        await _userRepository.UpdateProfileAsync(userId, request);
        return NoContent();
    }

    [HttpPut("admin/{userId}")]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> SetAdminStatus(Guid userId, [FromBody] bool isAdmin)
    {
        await _userRepository.SetAdminStatusAsync(userId, isAdmin);
        return NoContent();
    }

    //[Authorize]
    [HttpPost("upload-avatar")]
    public async Task<IActionResult> UploadAvatar(IFormFile file)
    {
        try
        {
            // 1. Проверка наличия файла
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            // 2. Проверка типа файла
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                return BadRequest("Invalid file type. Only JPG, JPEG and PNG are allowed.");

            // 3. Проверка размера файла (5MB)
            if (file.Length > 10 * 1024 * 1024)
                return BadRequest("File size exceeds limit (5MB)");

            // 4. Получение ID пользователя
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // 5. Генерация уникального имени файла
            var fileName = $"{userId}_{Guid.NewGuid()}{extension}";
            var uploadsFolder = Path.Combine("wwwroot", "avatars");
            var filePath = Path.Combine(uploadsFolder, fileName);

            // 6. Создание папки, если не существует
            Directory.CreateDirectory(uploadsFolder);

            // 7. Сохранение файла
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 8. Обновление аватара в БД
            var avatarUrl = $"/avatars/{fileName}";
            await _userRepository.UpdateAvatarAsync(userId, avatarUrl);

            return Ok(new { AvatarUrl = avatarUrl });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}