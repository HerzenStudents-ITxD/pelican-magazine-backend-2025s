using Backend.Models;

namespace Backend.Contracts.Responses;

public class AgeCategoryResponse
{
    public Guid AgeCategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;

    public AgeCategoryResponse(DbAgeCategory ageCategory)
    {
        AgeCategoryId = ageCategory.AgeCategoryId;
        CategoryName = ageCategory.CategoryName;
    }
}