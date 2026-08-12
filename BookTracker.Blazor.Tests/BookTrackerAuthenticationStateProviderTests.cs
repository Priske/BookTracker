using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BookTracker.Blazor.Auth;

namespace BookTracker.Blazor.Tests.Auth;

public class BookTrackerAuthenticationStateProviderTests
{
    [Fact]
    public async Task NoTokenReturnsAnonymousUser()
    {
        var authSession = new FakeAuthSession();

        var provider =
            new BookTrackerAuthenticationStateProvider(authSession);

        var state =
            await provider.GetAuthenticationStateAsync();

        Assert.False(
            state.User.Identity?.IsAuthenticated);
    }

    [Fact]
    public async Task ValidTokenReturnsAuthenticatedUser()
    {
        var token = CreateToken(
            expires: DateTime.UtcNow.AddMinutes(30),
            role: "Member");

        var authSession = new FakeAuthSession
        {
            Token = token
        };

        var provider =
            new BookTrackerAuthenticationStateProvider(authSession);

        var state =
            await provider.GetAuthenticationStateAsync();

        Assert.True(
            state.User.Identity?.IsAuthenticated);

        Assert.True(
            state.User.IsInRole("Member"));
    }

    [Fact]
    public async Task ValidTokenContainsExpectedClaims()
    {
        var token = CreateToken(
            expires: DateTime.UtcNow.AddMinutes(30),
            role: "Administrator");

        var authSession = new FakeAuthSession
        {
            Token = token
        };

        var provider =
            new BookTrackerAuthenticationStateProvider(authSession);

        var state =
            await provider.GetAuthenticationStateAsync();

        Assert.Equal(
            "Ada Reader",
            state.User.Identity?.Name);

        Assert.Equal(
            "reader@example.com",
            state.User.FindFirst(ClaimTypes.Email)?.Value);

        Assert.Equal(
            "42",
            state.User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value);

        Assert.True(
            state.User.IsInRole("Administrator"));
    }

    [Fact]
    public async Task ExpiredTokenReturnsAnonymousUser()
    {
        var token = CreateToken(
            expires: DateTime.UtcNow.AddMinutes(-30),
            role: "Member");

        var authSession = new FakeAuthSession
        {
            Token = token
        };

        var provider =
            new BookTrackerAuthenticationStateProvider(authSession);

        var state =
            await provider.GetAuthenticationStateAsync();

        Assert.False(
            state.User.Identity?.IsAuthenticated);
    }

    [Fact]
    public async Task InvalidTokenReturnsAnonymousUser()
    {
        var authSession = new FakeAuthSession
        {
            Token = "this-is-not-a-jwt"
        };

        var provider =
            new BookTrackerAuthenticationStateProvider(authSession);

        var state =
            await provider.GetAuthenticationStateAsync();

        Assert.False(
            state.User.Identity?.IsAuthenticated);
    }

    [Fact]
    public async Task SignOutChangesAuthenticationState()
    {
        var token = CreateToken(
            expires: DateTime.UtcNow.AddMinutes(30),
            role: "Member");

        var authSession = new FakeAuthSession
        {
            Token = token
        };

        var provider =
            new BookTrackerAuthenticationStateProvider(authSession);

        var authenticationStateChanged = false;

        provider.AuthenticationStateChanged += _ =>
        {
            authenticationStateChanged = true;
        };

        await provider.SignOutAsync();

        Assert.True(authenticationStateChanged);
        Assert.Null(authSession.Token);

        var state =
            await provider.GetAuthenticationStateAsync();

        Assert.False(
            state.User.Identity?.IsAuthenticated);
    }

    private static string CreateToken(
        DateTime expires,
        string role)
    {
        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                "42"),

            new Claim(
                ClaimTypes.Name,
                "Ada Reader"),

            new Claim(
                ClaimTypes.Email,
                "reader@example.com"),

            new Claim(
                ClaimTypes.Role,
                role)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: expires);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }

    private sealed class FakeAuthSession : IAuthSession
    {
        public string? Token { get; set; }

        public Task<string?> GetToken()
        {
            return Task.FromResult(Token);
        }

        public Task SaveToken(string token)
        {
            Token = token;

            return Task.CompletedTask;
        }

        public Task RemoveToken()
        {
            Token = null;

            return Task.CompletedTask;
        }
    }
}