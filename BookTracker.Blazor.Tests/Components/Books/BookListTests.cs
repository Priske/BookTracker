using System.Net;
using System.Net.Http.Json;
using Bunit;
using BookTracker.Blazor.Api;
using BookTracker.Blazor.Components.Books;
using BookTracker.Blazor.Models.Books;
using BookTracker.Blazor.Tests.TestHelpers;
using Microsoft.Extensions.DependencyInjection;

namespace BookTracker.Blazor.Tests.Components.Books;

public class BookListTests : BunitContext
{
    [Fact]
    public void ShowsLoadingWhileRequestIsPending()
    {
        var responseSource =
            new TaskCompletionSource<HttpResponseMessage>();

        var handler = new TestHttpMessageHandler(
            (_, _) => responseSource.Task);

        RegisterClient(handler);

        var cut = Render<BookList>();

        Assert.Contains("Boeken laden...", cut.Markup);

        responseSource.SetResult(CreateResponse([]));

        cut.WaitForAssertion(() =>
            Assert.DoesNotContain("Boeken laden...", cut.Markup));
    }

    [Fact]
    public void ShowsNoBooksWhenResultsAreEmpty()
    {
        var handler = new TestHttpMessageHandler(
            (_, _) => Task.FromResult(CreateResponse([])));

        RegisterClient(handler);

        var cut = Render<BookList>();

        cut.WaitForAssertion(() =>
        {
            Assert.DoesNotContain("Boeken laden...", cut.Markup);
            Assert.Empty(cut.FindAll("tr"));
        });
    }

    [Fact]
    public void ShowsErrorWhenRequestFails()
    {
        var handler = new TestHttpMessageHandler(
            (_, _) => Task.FromResult(
                new HttpResponseMessage(
                    HttpStatusCode.InternalServerError)));

        RegisterClient(handler);

        var cut = Render<BookList>();

        cut.WaitForAssertion(() =>
            Assert.Contains(
                "De boeken konden niet geladen worden.",
                cut.Markup));
    }

    [Fact]
    public void PreviousButtonIsDisabledOnFirstPage()
    {
        var handler = new TestHttpMessageHandler(
            (_, _) => Task.FromResult(
                CreateResponse(
                    [
                        new BookSummary
                        {
                            Id = 1,
                            Title = "Dune",
                            Author = "Frank Herbert"
                        }
                    ],
                    page: 1,
                    totalPages: 3)));

        RegisterClient(handler);

        var cut = Render<BookList>();

        cut.WaitForAssertion(() =>
            Assert.Contains("Dune", cut.Markup));

        var previousButton = cut.FindAll("button")
            .Single(button =>
                button.TextContent.Contains("Previous"));

        Assert.True(previousButton.HasAttribute("disabled"));
    }

    [Fact]
    public void NextButtonIsEnabledWhenMorePagesExist()
    {
        var handler = new TestHttpMessageHandler(
            (_, _) => Task.FromResult(
                CreateResponse(
                    [
                        new BookSummary
                        {
                            Id = 1,
                            Title = "Dune",
                            Author = "Frank Herbert"
                        }
                    ],
                    page: 1,
                    totalPages: 3)));

        RegisterClient(handler);

        var cut = Render<BookList>();

        cut.WaitForAssertion(() =>
            Assert.Contains("Dune", cut.Markup));

        var nextButton = cut.FindAll("button")
            .Single(button =>
                button.TextContent.Contains("Next"));

        Assert.False(nextButton.HasAttribute("disabled"));
    }

    private void RegisterClient(HttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost")
        };

        Services.AddSingleton(
            new BookTrackerClient(httpClient));
    }

    private static HttpResponseMessage CreateResponse(
        IReadOnlyList<BookSummary> books,
        int page = 1,
        int totalPages = 1)
    {
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(
                new GetBookSummariesResponse
                {
                    Items = books,
                    Page = page,
                    PageSize = 10,
                    TotalItems = books.Count,
                    TotalPages = totalPages
                })
        };
    }
}