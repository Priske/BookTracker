using System.Net.Http.Headers;
using BookTracker.Blazor.Auth;

namespace BookTracker.Blazor.Api;

public sealed class AuthorizationMessageHandler(
    IAuthSession authSession)
    : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await authSession.GetToken();

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token);
        }

        return await base.SendAsync(
            request,
            cancellationToken);
    }
}