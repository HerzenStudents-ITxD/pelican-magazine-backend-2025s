namespace Backend.Contracts.Requests;

public class RejectArticleRequest
{
    public string Comment { get; set; } = string.Empty;
    public List<HighlightedPart> HighlightedParts { get; set; } = new();
}

public class HighlightedPart
{
    public string Text { get; set; } = string.Empty;
    public int LineNumber { get; set; }
}