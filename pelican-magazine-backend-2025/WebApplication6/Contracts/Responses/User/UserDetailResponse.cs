namespace Backend.Contracts.Responses.User;

public class UserDetailResponse
{
    public required Guid UserId { get; set; }
    public required string Name { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public DateTime Birth { get; set; }
    public string? ProfileImg { get; set; }
    public string? ProfileCover { get; set; }
    public bool IsAdmin { get; set; }
}