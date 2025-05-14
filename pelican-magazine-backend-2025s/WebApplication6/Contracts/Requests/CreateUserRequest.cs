using System.ComponentModel.DataAnnotations;

namespace Backend.Contracts.Requests;

public class CreateUserRequest
{
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public string LastName { get; set; } = string.Empty;
    [EmailAddress][Required] public string Email { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
    [Required] public DateTime Birth { get; set; }
    [Required] public string Sex { get; set; } = "unknown"; // Добавляем обязательное поле
}