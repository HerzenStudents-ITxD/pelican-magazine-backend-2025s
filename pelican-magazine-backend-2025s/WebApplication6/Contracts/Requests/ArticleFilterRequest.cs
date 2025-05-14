namespace Backend.Contracts.Requests;

public class ArticleFilterRequest
{
    public string? Search { get; set; }
    public string? SortOrder { get; set; } // "asc" или "desc"
}