using System.ComponentModel.DataAnnotations;

namespace Backend.Contracts.Requests;

public class CreateAgeCategoryRequest
{
    [Required(ErrorMessage = "Название категории обязательно")]
    [StringLength(100, ErrorMessage = "Название не должно превышать 100 символов")]
    public string CategoryName { get; set; } = string.Empty;
}