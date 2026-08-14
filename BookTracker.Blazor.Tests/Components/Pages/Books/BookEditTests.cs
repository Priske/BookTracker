using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using Bunit;
using BookTracker.Blazor.Api;
using BookTracker.Blazor.Models.Books;
using BookTracker.Blazor.Pages.Books;
using BookTracker.Blazor.Tests.TestHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BookTracker.Blazor.Tests.Pages.Books;

public sealed class BookEditTests : BunitContext
{
    [Fact]
    public void ExistingBookDataIsFilledIn()
    {
        var version = Guid.NewGuid();

        var handler = new TestHttpMessageHandler((request, _) =>
        {
            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.Equal("/books/42", request.RequestUri?.AbsolutePath);

            return Task.FromResult(
                BookResponse(
                    "Dune",
                    "Frank Herbert",
                    1965,
                    version));
        });

        RegisterClient(handler);

        var cut = Render<BookEdit>(parameters => parameters
            .Add(component => component.Id, 42));

        cut.WaitForAssertion(() =>
        {
            var inputs = cut.FindAll("input");

            Assert.Equal("Dune", inputs[0].GetAttribute("value"));
            Assert.Equal("Frank Herbert", inputs[1].GetAttribute("value"));
            Assert.Equal("1965", inputs[2].GetAttribute("value"));
        });
    }

    [Fact]
    public void SuccessfulUpdateNavigatesToBookDetails()
    {
        var version = Guid.NewGuid();
        var call = 0;

        var handler = new TestHttpMessageHandler((request, _) =>
        {
            call++;

            if (call == 1)
            {
                Assert.Equal(HttpMethod.Get, request.Method);

                return Task.FromResult(
                    BookResponse(
                        "Dune",
                        "Frank Herbert",
                        1965,
                        version));
            }

            Assert.Equal(HttpMethod.Put, request.Method);

            return Task.FromResult(
                new HttpResponseMessage(HttpStatusCode.NoContent));
        });

        RegisterClient(handler);

        var navigationManager =
            Services.GetRequiredService<NavigationManager>();

        var cut = Render<BookEdit>(parameters => parameters
            .Add(component => component.Id, 42));

        cut.WaitForElement("form");

        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
            Assert.EndsWith(
                "/books/42",
                navigationManager.Uri));
    }

    [Fact]
    public void NotFoundIsShown()
    {
        var handler = new TestHttpMessageHandler((request, _) =>
        {
            Assert.Equal(HttpMethod.Get, request.Method);

            return Task.FromResult(
                new HttpResponseMessage(HttpStatusCode.NotFound));
        });

        RegisterClient(handler);

        var cut = Render<BookEdit>(parameters => parameters
            .Add(component => component.Id, 42));

        cut.WaitForAssertion(() =>
            Assert.Contains(
                "Book with id 42 was not found.",
                cut.Markup));
    }

    [Fact]
    public void ConflictIsShown()
    {
        var versionA = Guid.NewGuid();
        var versionB = Guid.NewGuid();

        var call = 0;

        var handler = new TestHttpMessageHandler((request, _) =>
        {
            call++;

            if (call == 1)
            {
                Assert.Equal(HttpMethod.Get, request.Method);

                return Task.FromResult(
                    BookResponse(
                        "Dune",
                        "Frank Herbert",
                        1965,
                        versionA));
            }

            if (call == 2)
            {
                Assert.Equal(HttpMethod.Put, request.Method);

                return Task.FromResult(
                    new HttpResponseMessage(HttpStatusCode.Conflict)
                    {
                        Content = JsonContent.Create(new
                        {
                            error = "The book was changed by another user."
                        })
                    });
            }

            Assert.Equal(HttpMethod.Get, request.Method);

            return Task.FromResult(
                BookResponse(
                    "Dune Updated",
                    "Frank Herbert",
                    1966,
                    versionB));
        });

        RegisterClient(handler);

        var cut = Render<BookEdit>(parameters => parameters
            .Add(component => component.Id, 42));

        cut.WaitForElement("form");

        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
            Assert.Contains(
                "The book was changed by another user.",
                cut.Markup));
    }

    [Fact]
    public void ConflictReloadsLatestVersionAndNextUpdateUsesIt()
    {
        var versionA = Guid.NewGuid();
        var versionB = Guid.NewGuid();

        var call = 0;

        string? firstPutBody = null;
        string? secondPutBody = null;

        var handler = new TestHttpMessageHandler(async (request, _) =>
        {
            call++;

            if (call == 1)
            {
                Assert.Equal(HttpMethod.Get, request.Method);

                return BookResponse(
                    "Dune",
                    "Frank Herbert",
                    1965,
                    versionA);
            }

            if (call == 2)
            {
                Assert.Equal(HttpMethod.Put, request.Method);

                firstPutBody =
                    await request.Content!.ReadAsStringAsync();

                return new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = JsonContent.Create(new
                    {
                        error = "The book was changed by another user."
                    })
                };
            }

            if (call == 3)
            {
                Assert.Equal(HttpMethod.Get, request.Method);

                return BookResponse(
                    "Dune Updated",
                    "Frank Herbert",
                    1966,
                    versionB);
            }

            Assert.Equal(HttpMethod.Put, request.Method);

            secondPutBody =
                await request.Content!.ReadAsStringAsync();

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        });

        RegisterClient(handler);

        var cut = Render<BookEdit>(parameters => parameters
            .Add(component => component.Id, 42));

        cut.WaitForElement("form");

        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
            Assert.NotNull(firstPutBody));

        using (var firstJson = JsonDocument.Parse(firstPutBody!))
        {
            var firstSentVersion =
                firstJson.RootElement
                    .GetProperty("version")
                    .GetGuid();

            Assert.Equal(versionA, firstSentVersion);
        }

        cut.WaitForAssertion(() =>
        {
            var inputs = cut.FindAll("input");

            Assert.Equal(
                "Dune Updated",
                inputs[0].GetAttribute("value"));

            Assert.Equal(
                "1966",
                inputs[2].GetAttribute("value"));
        });

        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
            Assert.NotNull(secondPutBody));

        using var secondJson =
            JsonDocument.Parse(secondPutBody!);

        var secondSentVersion =
            secondJson.RootElement
                .GetProperty("version")
                .GetGuid();

        Assert.Equal(versionB, secondSentVersion);
    }

    [Fact]
    public void BookEditRequiresAdministratorRole()
    {
        var authorizeAttribute =
            typeof(BookEdit)
                .GetCustomAttributes<AuthorizeAttribute>()
                .Single();

        Assert.Equal(
            "Administrator",
            authorizeAttribute.Roles);
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

    private static HttpResponseMessage BookResponse(
        string title,
        string author,
        int year,
        Guid version)
    {
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new
            {
                title,
                author,
                year,
                version
            })
        };
    }
}