namespace BookTracker.Blazor.Models.Books;

public enum UpdateBookStatus
{
    Updated,
    NotFound,
    Conflict,
    Unauthorized,
    Forbidden
}
public sealed record UpdateBookResult(UpdateBookStatus Status, string? ErrorMessage = null);
