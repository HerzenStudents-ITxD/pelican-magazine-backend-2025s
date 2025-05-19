using System.ComponentModel.DataAnnotations;

namespace Backend.Contracts.Requests.Auth;

public class Verify2FaRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "2FA code is required")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "2FA code must be 6 digits")]
    public string Code { get; set; } = string.Empty;
}