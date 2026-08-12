using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
//using System.Text;
//using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace BookTracker.Blazor.Auth;

public sealed class BookTrackerAuthenticationStateProvider
    : AuthenticationStateProvider
{
    private readonly IAuthSession _authSession;

    public BookTrackerAuthenticationStateProvider(IAuthSession authSession)
    {
        _authSession = authSession;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _authSession.GetToken();

        if (string.IsNullOrWhiteSpace(token))
        {
            return CreateAnonymousState();
        }

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            if (jwt.ValidTo <= DateTime.UtcNow)
            {
                return CreateAnonymousState();
            }

            var identity = new ClaimsIdentity(
                jwt.Claims,
                authenticationType: "jwt",
                nameType: ClaimTypes.Name,
                roleType: ClaimTypes.Role);
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);

            /*         Example of fully manual Jwt handling

          var claims = ParseClaimsFromJwt(token);
          if (IsExpired(claims))
          {
              return CreateAnonymousState();
          }

          var identity = new ClaimsIdentity(
              claims,
              authenticationType: "jwt");

          var user = new ClaimsPrincipal(identity);

          return new AuthenticationState(user);
          */
        }
        catch
        {
            return CreateAnonymousState();
        }
    }

    public async Task SignInAsync(string token)
    {
        await _authSession.SaveToken(token);

        NotifyAuthenticationStateChanged(
            GetAuthenticationStateAsync());
    }

    public async Task SignOutAsync()
    {
        await _authSession.RemoveToken();

        NotifyAuthenticationStateChanged(
            GetAuthenticationStateAsync());
    }

    private static AuthenticationState CreateAnonymousState()
    {
        var identity = new ClaimsIdentity();
        var user = new ClaimsPrincipal(identity);

        return new AuthenticationState(user);
    }
    /*
    private static List<Claim> ParseClaimsFromJwt(string token)
    {
        var parts = token.Split('.');

        if (parts.Length != 3)
        {
            throw new InvalidOperationException("Invalid JWT.");
        }

        var payload = parts[1];

        payload = payload
            .Replace('-', '+')
            .Replace('_', '/');

        switch (payload.Length % 4)
        {
            case 2:
                payload += "==";
                break;
            case 3:
                payload += "=";
                break;
        }

        var bytes = Convert.FromBase64String(payload);
        var json = Encoding.UTF8.GetString(bytes);

        using var document = JsonDocument.Parse(json);

        var claims = new List<Claim>();

        foreach (var property in document.RootElement.EnumerateObject())
        {
            var claimType = property.Name switch
            {
                "nameid" => ClaimTypes.NameIdentifier,
                "unique_name" => ClaimTypes.Name,
                "email" => ClaimTypes.Email,
                "role" => ClaimTypes.Role,
                _ => property.Name
            };

            if (property.Value.ValueKind == JsonValueKind.Array)
            {
                foreach (var value in property.Value.EnumerateArray())
                {
                    claims.Add(
                        new Claim(
                            claimType,
                            value.ToString()));
                }
            }
            else
            {
                claims.Add(
                    new Claim(
                        claimType,
                        property.Value.ToString()));
            }
        }

        return claims;
    }
    

    private static bool IsExpired(IEnumerable<Claim> claims)
    {
        var expirationClaim =
            claims.FirstOrDefault(claim => claim.Type == "exp");

        if (expirationClaim is null)
        {
            return true;
        }

        if (!long.TryParse(expirationClaim.Value, out var expirationSeconds))
        {
            return true;
        }

        var expiration =
            DateTimeOffset.FromUnixTimeSeconds(expirationSeconds);

        return expiration <= DateTimeOffset.UtcNow;
    }
    */
}