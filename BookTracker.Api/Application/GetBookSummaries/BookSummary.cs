namespace BookTracker.Api.Application.GetBookSummaries;
//This is a DTO
public class BookSummary
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Author { get; set; }
}