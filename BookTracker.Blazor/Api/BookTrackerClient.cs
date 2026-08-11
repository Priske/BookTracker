using System.Net.Http.Json;
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
}