using System.Security.Claims;
using BookTracker.Api.Application.Auth.GetCurrentMember;
using BookTracker.Api.Application.Auth.Login;

namespace BookTracker.Api.Endpoints.Auth;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/login", Login);

        app.MapGet("/auth/me", GetCurrentMember)
            .RequireAuthorization();

        return app;
    }

    private static async Task<IResult> Login(
        LoginRequest request,
        LoginCommandHandler handler)
    {
        var response = await handler.Execute(request);

        if (response is null)
        {
            return Results.Unauthorized();
        }

        return Results.Ok(response);
    }

    private static async Task<IResult> GetCurrentMember(
     ClaimsPrincipal principal,
     GetCurrentMemberQueryHandler handler)
    {
        var idValue = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(idValue, out var id))
        {
            return Results.Unauthorized();
        }

        var member = await handler.Execute(id);

        if (member is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(member);
    }
}