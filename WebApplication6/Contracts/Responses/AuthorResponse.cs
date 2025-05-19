using Backend.Models;

namespace Backend.Contracts.Responses;

public class AuthorResponse
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Nickname { get; set; }
    public string? ProfileImage { get; set; }

    public AuthorResponse(DbUser user)
    {
        UserId = user.UserId;
        Name = user.Name;
        LastName = user.LastName;
        Nickname = user.Nickname;
        ProfileImage = user.ProfileImg;
    }
}