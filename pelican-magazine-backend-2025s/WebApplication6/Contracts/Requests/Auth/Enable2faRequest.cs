using System.ComponentModel.DataAnnotations;

namespace Backend.Contracts.Requests.Auth;

public class Enable2faRequest
{
    [Required]
    public string Code { get; set; } = string.Empty;
}