using BookTracker.Api.Application.Members.CreateMember;
using BookTracker.Api.Domain;
using BookTracker.Api.Application.Members.GetMemberSummaries;
using BookTracker.Api.Application.Members.UpdateMember;
using BookTracker.Api.Application.Members.DeleteMember;
using BookTracker.Api.Application.Members.GetMemberDetails;

using System.Security.Claims;
using BookTracker.Api.Domain.Members;
namespace BookTracker.Api.Endpoints.Members;


public static class MemberEndpoints
{
    public static IEndpointRouteBuilder MapMemberEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/members", GetMemberSummaries)
            .RequireAuthorization();

        app.MapGet("/members/{id:int}", GetMemberDetails)
            .RequireAuthorization();
        app.MapPost("/members", CreateMember);

        app.MapPut("/members/{id:int}", UpdateMember)
            .RequireAuthorization();

        app.MapDelete("/members/{id:int}", DeleteMember)
            .RequireAuthorization();

        return app;
    }

    public static async Task<IResult> GetMemberSummaries(
       [AsParameters] GetMemberSummariesRequest request,
       ClaimsPrincipal principal,
       GetMemberSummariesQueryHandler handler)
    {
        try
        {
            var actor = principal.ToActor();
            var response = await handler.Execute(actor, request);

            return Results.Ok(response);
        }
        catch (ForbiddenOperationException)
        {
            return Results.Forbid();
        }
    }

    public static async Task<IResult> GetMemberDetails(
        int id,
        ClaimsPrincipal principal,
        GetMemberDetailsQueryHandler query)
    {
        try
        {
            var actor = principal.ToActor();
            var member = await query.Execute(actor, id);

            if (member is null)
            {
                return Results.NotFound();
            }
            return Results.Ok(member);
        }
        catch (ForbiddenOperationException)
        {
            return Results.Forbid();
        }
        catch (DomainException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }

    }
    public static async Task<IResult> CreateMember(
        CreateMemberRequest request,
        CreateMemberCommandHandler handler)
    {
        try
        {
            var response = await handler.Execute(request);
            return Results.Created($"/members/{response.Id}", response);
        }
        catch (MemberEmailAlreadyExistsException exception)
        {
            return Results.Conflict(new { error = exception.Message });
        }
        catch (DomainException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }

    }
    public static async Task<IResult> UpdateMember(
        int id,
        ClaimsPrincipal principal,
        UpdateMemberRequest request,
        UpdateMemberCommandHandler handler)
    {
        try
        {
            var actor = principal.ToActor();
            var updated = await handler.Execute(actor, id, request);

            if (!updated)
            {
                return Results.NotFound();
            }
            return Results.NoContent();
        }
        catch (ForbiddenOperationException)
        {
            return Results.Forbid();
        }
        catch (MemberEmailAlreadyExistsException exception)
        {
            return Results.Conflict(new { error = exception.Message });
        }
        catch (DomainException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }
    }

    public static async Task<IResult> DeleteMember(
        int id,
        ClaimsPrincipal principal,
        DeleteMemberCommandHandler handler)
    {
        try
        {
            var actor = principal.ToActor();
            var deleted = await handler.Execute(actor, id);

            if (!deleted)
            {
                return Results.NotFound();
            }

            return Results.NoContent();
        }
        catch (ForbiddenOperationException)
        {
            return Results.Forbid();
        }
        catch (DomainException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }

    }

}