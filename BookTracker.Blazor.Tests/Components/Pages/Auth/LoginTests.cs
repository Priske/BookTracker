using System.Net;
using System.Net.Http.Json;
using Bunit;
using BookTracker.Blazor.Api;
using BookTracker.Blazor.Auth;
using BookTracker.Blazor.Models.Auth;
using BookTracker.Blazor.Pages.Auth;
using BookTracker.Blazor.Tests.TestHelpers;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace BookTracker.Blazor.Tests.Components.Pages.Auth;

public class LoginTests : BunitContext
{
    [Fact]
    public void InvalidCredentialsShowsError()
    {
        var handler = new TestHttpMessageHandler((_, _) =>
            Task.FromResult(
                new HttpResponseMessage(HttpStatusCode.Unauthorized)));

        RegisterClient(handler);

        var authSession = new FakeAuthSession();
        Services.AddSingleton<IAuthSession>(authSession);

        var cut = Render<Login>();

        cut.Find("#email").Change("test@example.com");
        cut.Find("#password").Change("wrong-password");

        cut.Find("button[type='submit']").Click();

        cut.WaitForAssertion(() =>
            Assert.Contains(
                "Invalid email or password.",
                cut.Markup));
    }

    [Fact]
    public void SuccessfulLoginStoresTokenAndNavigatesAway()
    {
        var handler = new TestHttpMessageHandler((_, _) =>
            Task.FromResult(
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(
                        new LoginResponse
                        {
                            AccessToken = "test-token",
                            ExpiresAt = DateTime.UtcNow.AddHours(1)
                        })
                }));

        RegisterClient(handler);

        var authSession = new FakeAuthSession();
        Services.AddSingleton<IAuthSession>(authSession);

        var navigationManager =
            Services.GetRequiredService<NavigationManager>();

        var cut = Render<Login>();

        cut.Find("#email").Change("test@example.com");
        cut.Find("#password").Change("correct-password");

        cut.Find("button[type='submit']").Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Equal("test-token", authSession.Token);

            Assert.EndsWith(
                "/booktracker",
                navigationManager.Uri);
        });
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

    private sealed class FakeAuthSession : IAuthSession
    {
        public string? Token { get; private set; }

        public Task SaveToken(string token)
        {
            Token = token;
            return Task.CompletedTask;
        }

        public Task<string?> GetToken()
        {
            return Task.FromResult(Token);
        }

        public Task RemoveToken()
        {
            Token = null;
            return Task.CompletedTask;
        }
    }
}