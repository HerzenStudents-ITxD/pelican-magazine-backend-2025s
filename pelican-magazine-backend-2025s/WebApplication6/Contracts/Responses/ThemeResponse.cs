using Backend.Models;

namespace Backend.Contracts.Responses;

public class ThemeResponse
{
    public Guid ThemeId { get; set; }
    public string ThemeName { get; set; } = string.Empty;

    public ThemeResponse(DbTheme theme)
    {
        ThemeId = theme.ThemeId;
        ThemeName = theme.ThemeName;
    }
}