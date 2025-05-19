namespace Backend.Contracts.Responses;

public class ArticleFullResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Content { get; set; }
    public string Summary { get; set; }
    public string CoverImage { get; set; }
    public List<string> Categories { get; set; }
    public string AgeRestriction { get; set; }
    public AuthorResponse Author { get; set; }
    public DateTime CreatedAt { get; set; }
}