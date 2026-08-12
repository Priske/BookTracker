using System.Net;
using System.Net.Http.Json;
using Bunit;
using Bunit.TestDoubles;
using BookTracker.Blazor.Api;
using BookTracker.Blazor.Auth;
using BookTracker.Blazor.Models.Books;
using BookTracker.Blazor.Pages.Books;
using BookTracker.Blazor.Tests.TestHelpers;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace BookTracker.Blazor.Tests.Components.Pages.Books;

public class CreateBookTests : BunitContext
{
    [Fact]
    public void ValidSubmitSendsCreateBookRequest()
    {
        CreateBookRequest? sentRequest = null;

        var handler = new TestHttpMessageHandler(async (request, _) =>
        {
            sentRequest =
                await request.Content!
                    .ReadFromJsonAsync<CreateBookRequest>();

            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = JsonContent.Create(
                    new CreateBookResponse
                    {
                        Id = 42,
                        Title = "Dune",
                        Author = "Frank Herbert",
                        Year = 1965
                    })
            };
        });

        RegisterServices(handler);

        var authorization = AddAuthorization();
        authorization.SetAuthorized("Ada Reader");
        authorization.SetRoles("Administrator");

        var cut = Render<CreateBook>();

        cut.Find("#title").Change("Dune");
        cut.Find("#author").Change("Frank Herbert");
        cut.Find("#year").Change("1965");

        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
        {
            Assert.NotNull(sentRequest);
            Assert.Equal("Dune", sentRequest!.Title);
            Assert.Equal("Frank Herbert", sentRequest.Author);
            Assert.Equal(1965, sentRequest.Year);
        });
    }

    [Fact]
    public async Task SubmitButtonIsDisabledWhileRequestIsRunning()
    {
        var completion =
            new TaskCompletionSource<HttpResponseMessage>();

        var handler = new TestHttpMessageHandler(
            (_, _) => completion.Task);

        RegisterServices(handler);

        var authorization = AddAuthorization();
        authorization.SetAuthorized("Ada Reader");
        authorization.SetRoles("Administrator");

        var cut = Render<CreateBook>();

        cut.Find("#title").Change("Dune");
        cut.Find("#author").Change("Frank Herbert");
        cut.Find("#year").Change("1965");

        var submit =
            cut.Find("form")
                .SubmitAsync(new EventArgs());

        cut.WaitForAssertion(() =>
        {
            var button =
                cut.Find("button[type='submit']");

            Assert.True(
                button.HasAttribute("disabled"));
        });

        completion.SetResult(
            new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = JsonContent.Create(
                    new CreateBookResponse
                    {
                        Id = 42,
                        Title = "Dune",
                        Author = "Frank Herbert",
                        Year = 1965
                    })
            });

        await submit;
    }

    [Fact]
    public void BadRequestShowsServerValidationMessage()
    {
        var handler = new TestHttpMessageHandler((_, _) =>
            Task.FromResult(
                new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = JsonContent.Create(
                        new
                        {
                            error = "Title is required."
                        })
                }));

        RegisterServices(handler);

        var authorization = AddAuthorization();
        authorization.SetAuthorized("Ada Reader");
        authorization.SetRoles("Administrator");

        var cut = Render<CreateBook>();

        cut.Find("#title").Change("Dune");
        cut.Find("#author").Change("Frank Herbert");
        cut.Find("#year").Change("1965");

        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains(
                "Title is required.",
                cut.Markup);
        });
    }

    [Fact]
    public void SuccessfulCreateNavigatesToBookDetails()
    {
        var handler = new TestHttpMessageHandler((_, _) =>
            Task.FromResult(
                new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = JsonContent.Create(
                        new CreateBookResponse
                        {
                            Id = 42,
                            Title = "Dune",
                            Author = "Frank Herbert",
                            Year = 1965
                        })
                }));

        RegisterServices(handler);

        var authorization = AddAuthorization();
        authorization.SetAuthorized("Ada Reader");
        authorization.SetRoles("Administrator");

        var navigationManager =
            Services.GetRequiredService<NavigationManager>();

        var cut = Render<CreateBook>();

        cut.Find("#title").Change("Dune");
        cut.Find("#author").Change("Frank Herbert");
        cut.Find("#year").Change("1965");

        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
        {
            Assert.EndsWith(
                "/books/42",
                navigationManager.Uri);
        });
    }

    [Fact]
    public void CreateBookPageRequiresAdministratorRole()
    {
        var authorizeAttribute =
            typeof(CreateBook)
                .GetCustomAttributes(
                    typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute),
                    inherit: true)
                .Cast<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>()
                .Single();

        Assert.Equal(
            "Administrator",
            authorizeAttribute.Roles);
    }

    private void RegisterServices(
        HttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler)
        {
            BaseAddress =
                new Uri("http://localhost")
        };

        Services.AddSingleton(
            new BookTrackerClient(httpClient));
    }
}