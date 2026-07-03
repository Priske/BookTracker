namespace BookTracker.Api.Application.BookList;
//This is a DTO
public class BookInfo
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Author { get; set; }
}