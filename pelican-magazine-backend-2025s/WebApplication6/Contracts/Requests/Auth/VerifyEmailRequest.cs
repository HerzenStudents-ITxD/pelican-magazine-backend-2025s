using System.ComponentModel.DataAnnotations;

namespace Backend.Contracts.Requests.Auth;

public class VerifyEmailRequest
{
    [Required]
    public string Token { get; set; } = string.Empty;
}

