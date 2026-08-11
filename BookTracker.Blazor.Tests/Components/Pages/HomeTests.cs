using System.Net;
using System.Net.Http.Json;
using Bunit;
using BookTracker.Blazor.Api;
using BookTracker.Blazor.Models.Books;
using BookTracker.Blazor.Pages;
using BookTracker.Blazor.Tests.TestHelpers;
using Microsoft.Extensions.DependencyInjection;

namespace BookTracker.Blazor.Tests.Components.Pages;

public class HomeTests : BunitContext
{
    [Fact]
    public void HidesAuthorsWhenToggleIsClicked()
    {
        RegisterClient(
        [
            new BookSummary
            {
                Id = 1,
                Title = "Dune",
                Author = "Frank Herbert"
            },
            new BookSummary
            {
                Id = 2,
                Title = "The Big Sleep",
                Author = "Raymond Chandler"
            }
        ]);

        var cut = Render<Home>();

        cut.WaitForAssertion(() =>
            Assert.Contains("Frank Herbert", cut.Markup));

        cut.Find("button").Click();

        Assert.DoesNotContain("Frank Herbert", cut.Markup);
        Assert.DoesNotContain("Raymond Chandler", cut.Markup);
    }


    private void RegisterClient(IReadOnlyList<BookSummary> books)
    {
        var handler = new TestHttpMessageHandler((_, _) =>
            Task.FromResult(
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(
                        new GetBookSummariesResponse
                        {
                            Items = books,
                            Page = 1,
                            PageSize = 10,
                            TotalItems = books.Count,
                            TotalPages = 1
                        })
                }));

        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost")
        };

        Services.AddSingleton(new BookTrackerClient(httpClient));
    }
}