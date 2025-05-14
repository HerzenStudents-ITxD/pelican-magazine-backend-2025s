using System.ComponentModel.DataAnnotations;

namespace Backend.Contracts.Requests.Auth;

public class RegisterRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public DateTime Birth { get; set; }

    [Required]
    public string Sex { get; set; } = "unknown";
}