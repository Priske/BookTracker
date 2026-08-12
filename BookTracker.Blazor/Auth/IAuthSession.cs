namespace BookTracker.Blazor.Auth;

public interface IAuthSession
{
    Task SaveToken(string token);

    Task<string?> GetToken();

    Task RemoveToken();
}