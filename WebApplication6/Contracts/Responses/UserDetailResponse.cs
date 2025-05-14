namespace Backend.Contracts.Responses;

public class UserDetailResponse
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime Birth { get; set; }
    public string? ProfileImg { get; set; }
    public string? ProfileCover { get; set; }
    public bool IsAdmin { get; set; }
}