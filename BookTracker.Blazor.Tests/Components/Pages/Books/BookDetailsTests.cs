using System.Net;
using System.Net.Http.Json;
using Bunit;
using BookTracker.Blazor.Api;
using BookTracker.Blazor.Models.Books;
using BookTracker.Blazor.Pages.Books;
using BookTracker.Blazor.Tests.TestHelpers;
using Microsoft.Extensions.DependencyInjection;

namespace BookTracker.Blazor.Tests.Components.Pages.Books;

public class BookDetailsTests : BunitContext
{
    [Fact]
    public void UsesRouteParameter()
    {
        var requestedUrl = "";

        var handler = new TestHttpMessageHandler((request, _) =>
        {
            requestedUrl = request.RequestUri!.PathAndQuery;

            return Task.FromResult(CreateBookResponse(
                id: 42,
                title: "Dune"));
        });

        RegisterClient(handler);

        var cut = Render<BookDetails>(parameters => parameters
            .Add(component => component.Id, 42));

        cut.WaitForAssertion(() =>
            Assert.Equal("/books/42", requestedUrl));
    }

    [Fact]
    public void ShowsNotFoundWhenBookDoesNotExist()
    {
        var handler = new TestHttpMessageHandler((_, _) =>
            Task.FromResult(
                new HttpResponseMessage(HttpStatusCode.NotFound)));

        RegisterClient(handler);

        var cut = Render<BookDetails>(parameters => parameters
            .Add(component => component.Id, 42));

        cut.WaitForAssertion(() =>
            Assert.Contains(
                "Book with id: 42 was not found",
                cut.Markup));
    }

    [Fact]
    public void ShowsBookWhenFound()
    {
        var handler = new TestHttpMessageHandler((_, _) =>
            Task.FromResult(CreateBookResponse(
                id: 42,
                title: "Dune")));

        RegisterClient(handler);

        var cut = Render<BookDetails>(parameters => parameters
            .Add(component => component.Id, 42));

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("Dune", cut.Markup);
            Assert.Contains("Frank Herbert", cut.Markup);
            Assert.Contains("1965", cut.Markup);
        });
    }

    [Fact]
    public void LoadsNewBookWhenIdChanges()
    {
        var handler = new TestHttpMessageHandler((request, _) =>
        {
            var id = request.RequestUri!.AbsolutePath.EndsWith("/43")
                ? 43
                : 42;

            return Task.FromResult(CreateBookResponse(
                id,
                $"Book {id}"));
        });

        RegisterClient(handler);

        var cut = Render<BookDetails>(parameters => parameters
            .Add(component => component.Id, 42));

        cut.WaitForAssertion(() =>
            Assert.Contains("Book 42", cut.Markup));

        cut.Render(parameters => parameters
            .Add(component => component.Id, 43));

        cut.WaitForAssertion(() =>
            Assert.Contains("Book 43", cut.Markup));
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

    private static HttpResponseMessage CreateBookResponse(
        int id,
        string title)
    {
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(
                new BookDetailsResponse
                {
                    Id = id,
                    Title = title,
                    Author = "Frank Herbert",
                    Year = 1965,
                    Version = Guid.NewGuid()
                })
        };
    }
}