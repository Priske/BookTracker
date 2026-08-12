namespace BookTracker.Blazor.Models.Auth;

public sealed class LoginResponse
{
    public required string AccessToken { get; set; }

    public DateTime ExpiresAt { get; set; }
}