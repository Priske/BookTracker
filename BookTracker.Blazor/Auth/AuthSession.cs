using Microsoft.JSInterop;

namespace BookTracker.Blazor.Auth;

public sealed class AuthSession(IJSRuntime jsRuntime) : IAuthSession
{
    private const string TokenKey = "booktracker-access-token";

    public async Task SaveToken(string token)
    {
        await jsRuntime.InvokeVoidAsync(
            "localStorage.setItem",
            TokenKey,
            token);
    }

    public async Task<string?> GetToken()
    {
        return await jsRuntime.InvokeAsync<string?>(
            "localStorage.getItem",
            TokenKey);
    }

    public async Task RemoveToken()
    {
        await jsRuntime.InvokeVoidAsync(
            "localStorage.removeItem",
            TokenKey);
    }
}