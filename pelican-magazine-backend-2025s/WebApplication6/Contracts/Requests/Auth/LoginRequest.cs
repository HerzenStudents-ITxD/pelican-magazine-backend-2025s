﻿using System.ComponentModel.DataAnnotations;

namespace Backend.Contracts.Requests.Auth;

public class LoginRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    public string? TwoFactorCode { get; set; } // Код 2FA
}
