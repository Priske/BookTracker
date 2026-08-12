using System.Net;
using BookTracker.Blazor.Api;
using BookTracker.Blazor.Auth;
using BookTracker.Blazor.Tests.TestHelpers;

namespace BookTracker.Blazor.Tests.Api;

public class AuthorizationMessageHandlerTests
{
    [Fact]
    public async Task AddsBearerTokenWhenTokenExists()
    {
        var authSession = new FakeAuthSession("test-token");

        HttpRequestMessage? sentRequest = null;

        var innerHandler = new TestHttpMessageHandler((request, _) =>
        {
            sentRequest = request;

            return Task.FromResult(
                new HttpResponseMessage(HttpStatusCode.OK));
        });

        var handler =
            new AuthorizationMessageHandler(authSession)
            {
                InnerHandler = innerHandler
            };

        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost")
        };

        await httpClient.GetAsync("/books");

        Assert.NotNull(sentRequest);
        Assert.NotNull(sentRequest.Headers.Authorization);

        Assert.Equal(
            "Bearer",
            sentRequest.Headers.Authorization.Scheme);

        Assert.Equal(
            "test-token",
            sentRequest.Headers.Authorization.Parameter);
    }

    private sealed class FakeAuthSession(string? token)
        : IAuthSession
    {
        public Task SaveToken(string token)
        {
            return Task.CompletedTask;
        }

        public Task<string?> GetToken()
        {
            return Task.FromResult(token);
        }

        public Task RemoveToken()
        {
            return Task.CompletedTask;
        }
    }
}