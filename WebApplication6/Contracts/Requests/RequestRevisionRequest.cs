namespace Backend.Contracts.Requests;

public class RequestRevisionRequest
{
    public string Comment { get; set; } = string.Empty;
    public List<HighlightedPart> HighlightedParts { get; set; } = new();
}