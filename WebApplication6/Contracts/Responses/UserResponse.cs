namespace Backend.Contracts.Responses;

public class UserResponse
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool TwoFactorEnabled { get; set; }
}