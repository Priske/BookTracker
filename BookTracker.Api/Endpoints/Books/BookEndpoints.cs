using BookTracker.Api.Application.Books.GetBookSummaries;
using BookTracker.Api.Application.Books.CreateBook;
using BookTracker.Api.Application.Books.DeleteBook;
using BookTracker.Api.Application.Books.GetBookDetails;
using BookTracker.Api.Application.Books.UpdateBook;
using BookTracker.Api.Domain;
using BookTracker.Api.Security;
using System.Security.Claims;

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
        try
        {
            var actor = principal.ToActor();

            var response = await handler.Execute(actor, request);

            return Results.Created($"/books/{response.Id}", response);
        }
        catch (ForbiddenOperationException)
        {
            return Results.Forbid();
        }
        catch (DomainException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }
    }
    public static async Task<IResult> UpdateBook(int id,
        UpdateBookRequest request,
        ClaimsPrincipal principal,
        UpdateBookCommandHandler handler)
    {
        try
        {
            var actor = principal.ToActor();
            var updated = await handler.Execute(actor, id, request);

            if (!updated)
            {
                return Results.NotFound();
            }
            return Results.NoContent();
        }
        catch (ForbiddenOperationException)
        {
            return Results.Forbid();
        }
        catch (DomainException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }
    }

    public static async Task<IResult> DeleteBook(int id,
        ClaimsPrincipal principal,
        DeleteBookCommandHandler handler)
    {
        try
        {
            var actor = principal.ToActor();
            var deleted = await handler.Execute(actor, id);

            if (!deleted)
            {
                return Results.NotFound();
            }

            return Results.NoContent();
        }
        catch (ForbiddenOperationException)
        {
            return Results.Forbid();
        }
        catch (DomainException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }

    }

}
