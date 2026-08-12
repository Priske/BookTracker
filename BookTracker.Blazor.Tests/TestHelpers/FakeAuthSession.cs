using BookTracker.Blazor.Auth;

namespace BookTracker.Blazor.Tests.TestHelpers;

public sealed class FakeAuthSession : IAuthSession
{
    public string? Token { get; set; }

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