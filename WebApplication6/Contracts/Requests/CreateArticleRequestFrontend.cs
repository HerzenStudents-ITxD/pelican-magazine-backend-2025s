namespace Backend.Contracts.Requests;

public class CreateArticleRequestFrontend
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Content { get; set; }
    public string Summary { get; set; }
    public List<string> Categories { get; set; }
    public string AgeRestriction { get; set; }
}