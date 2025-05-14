// CreateThemeRequest.cs
using System.ComponentModel.DataAnnotations;

namespace Backend.Contracts.Requests;

public class CreateThemeRequest
{
    [Required(ErrorMessage = "Название темы обязательно")]
    [StringLength(100, ErrorMessage = "Название не должно превышать 100 символов")]
    public string ThemeName { get; set; } = string.Empty;
}

