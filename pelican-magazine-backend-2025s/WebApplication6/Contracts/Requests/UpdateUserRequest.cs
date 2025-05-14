using Backend.Models;
using System.ComponentModel.DataAnnotations;
using static Backend.Models.DbUser;

namespace Backend.Contracts.Requests;

public class UpdateUserRequest
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, ErrorMessage = "Last name cannot be longer than 50 characters")]
    public string LastName { get; set; } = string.Empty;

    [StringLength(30, ErrorMessage = "Nickname cannot be longer than 30 characters")]
    public string? Nickname { get; set; }
    public int? Year { get; set; }
    public int? Course { get; set; }
    public string? Degree { get; set; }
    public UserType? UserType { get; set; }
}