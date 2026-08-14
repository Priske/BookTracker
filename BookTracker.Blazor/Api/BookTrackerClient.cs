using System.Net;
using System.Net.Http.Json;
using BookTracker.Blazor.Models;
using BookTracker.Blazor.Models.Auth;
using BookTracker.Blazor.Models.Books;

namespace BookTracker.Blazor.Api;

public sealed class BookTrackerClient(HttpClient httpClient)
{
    public async Task<GetBookSummariesResponse> GetBooks(
        string? search,
        int page,
        int pageSize)
    {
        var url = $"/books?page={page}&pageSize={pageSize}";

        if (!string.IsNullOrWhiteSpace(search))
        {
            // escaped Datastring makes special character into URLsafe notations  search = "C# & .NET"; becomes  C%23%20%26%20.NET
            url += $"&search={Uri.EscapeDataString(search)}";
        }

        return await httpClient.GetFromJsonAsync<GetBookSummariesResponse>(url)
            ?? throw new InvalidOperationException("Book list response was empty.");
    }

    public async Task<BookDetailsResponse?> GetBookDetails(int id)
    {
        using var response = await httpClient.GetAsync($"/books/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<BookDetailsResponse>()
            ?? throw new InvalidOperationException("Book details response was empty.");
    }

    public async Task<LoginResponse?> LoginUser(LoginRequest request)
    {
        using var response = await httpClient.PostAsJsonAsync("/auth/login", request);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<LoginResponse>()
            ?? throw new InvalidOperationException("Login response was empty.");
    }
    public async Task<UpdateBookResult> EditBook(
        int id,
        UpdateBookRequest request)
    {
        using var response =
            await httpClient.PutAsJsonAsync($"/books/{id}", request);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return new UpdateBookResult(
                UpdateBookStatus.Updated);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return new UpdateBookResult(
                UpdateBookStatus.NotFound);
        }

        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            var errorResponse =
                await response.Content.ReadFromJsonAsync<ErrorResponse>();

            return new UpdateBookResult(
                UpdateBookStatus.Conflict,
                errorResponse?.Error ?? "The book was changed by another user.");
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return new UpdateBookResult(
                UpdateBookStatus.Unauthorized);
        }

        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            return new UpdateBookResult(
                UpdateBookStatus.Forbidden);
        }

        response.EnsureSuccessStatusCode();

        throw new InvalidOperationException(
            "Unexpected update response.");
    }

    public async Task<CreateBookResult> CreateBook(CreateBookRequest request)
    {
        using var response =
            await httpClient.PostAsJsonAsync("/books", request);

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var errorResponse =
                await response.Content.ReadFromJsonAsync<ErrorResponse>();

            return new CreateBookResult(
                CreateBookStatus.ValidationFailed,
                ErrorMessage:
                    errorResponse?.Error
                    ?? "Invalid book data.");
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return new CreateBookResult(
                CreateBookStatus.Unauthorized);
        }

        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            return new CreateBookResult(
                CreateBookStatus.Forbidden);
        }

        response.EnsureSuccessStatusCode();

        var book =
            await response.Content
                .ReadFromJsonAsync<CreateBookResponse>()
            ?? throw new InvalidOperationException(
                "Create book response was empty.");

        return new CreateBookResult(
            CreateBookStatus.Created,
            book);
    }

}