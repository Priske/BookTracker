namespace BookTracker.Blazor.Models.Books;

public enum CreateBookStatus
{
    Created,
    ValidationFailed,
    Unauthorized,
    Forbidden
}

public sealed record CreateBookResult(
    CreateBookStatus Status,
    CreateBookResponse? Book = null,
    string? ErrorMessage = null);