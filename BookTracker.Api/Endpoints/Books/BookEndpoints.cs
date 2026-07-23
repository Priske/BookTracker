using BookTracker.Api.Application.Books.GetBookSummaries;
using BookTracker.Api.Application.Books.CreateBook;
using BookTracker.Api.Application.Books.DeleteBook;
using BookTracker.Api.Application.Books.GetBookDetails;
using BookTracker.Api.Application.Books.UpdateBook;
using BookTracker.Api.Domain;
using System.Security.Claims;
using BookTracker.Api.Storage.Books;

namespace BookTracker.Api.Endpoints.Books;

public static class BookEndpoints
{
    public static IEndpointRouteBuilder MapBookEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/books", GetBookSummaries);
        app.MapGet("/books/{id:int}", GetBookDetails);

        app.MapPost("/books", CreateBook)
            .RequireAuthorization();

        app.MapPut("/books/{id:int}", UpdateBook)
            .RequireAuthorization();

        app.MapDelete("/books/{id:int}", DeleteBook)
            .RequireAuthorization();

        return app;
    }

    public static async Task<IResult> GetBookSummaries(
        [AsParameters] GetBookSummariesRequest request,
        GetBookSummariesQueryHandler query)
    {
        var books = await query.Execute(request);

        return Results.Ok(books);
    }


    public static async Task<IResult> GetBookDetails(int id, GetBookDetailsQueryHandler query)
    {
        var book = await query.Execute(id);

        if (book is null)
        {
            return Results.NotFound();
        }
        return Results.Ok(book);
    }
    public static async Task<IResult> CreateBook(
        CreateBookRequest request,
        ClaimsPrincipal principal,
        CreateBookCommandHandler handler)
    {
        var response = await handler.Execute(principal.ToActor(), request);
        return Results.Created($"/books/{response.Id}", response);
    }
    public static async Task<IResult> UpdateBook(int id,
        UpdateBookRequest request,
        ClaimsPrincipal principal,
        UpdateBookCommandHandler handler)
    {
        var result = await handler.Execute(principal.ToActor(), id, request);

        return result switch
        {
            UpdateBookResult.Updated => Results.NoContent(),
            UpdateBookResult.NotFound => Results.NotFound(),
            UpdateBookResult.Conflict => Results.Conflict(
                new
                {
                    error = "The book was changed by another user."
                }),
            _ => throw new ArgumentOutOfRangeException()
        };
    }


    public static async Task<IResult> DeleteBook(int id,
        ClaimsPrincipal principal,
        DeleteBookCommandHandler handler)
    {
        var deleted = await handler.Execute(principal.ToActor(), id);
        return deleted ? Results.NoContent() : Results.NotFound();
    }


}
