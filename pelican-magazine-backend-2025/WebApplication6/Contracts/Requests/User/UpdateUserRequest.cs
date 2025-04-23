using System.ComponentModel.DataAnnotations;

namespace Backend.Contracts.Requests.User;

public class UpdateUserRequest
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    [Required]
    [StringLength(50)]
    public string LastName { get; set; }

    [StringLength(30)]
    public string? Nickname { get; set; }
}