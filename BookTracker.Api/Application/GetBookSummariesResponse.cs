namespace BookTracker.Api.Application;

public class GetBookSummariesResponse<T>
{
    public required IReadOnlyList<T> Items { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalItems { get; set; }

    public int TotalPages { get; set; }
}