using BookTracker.Api.Application.Members.CreateMember;
using BookTracker.Api.Domain;
using BookTracker.Api.Application.Members.GetMemberSummaries;
using BookTracker.Api.Application.Members.UpdateMember;
using BookTracker.Api.Application.Members.DeleteMember;
using BookTracker.Api.Application.Members.GetMemberDetails;
using BookTracker.Api.Application.Members;
using System.Security.Claims;
namespace BookTracker.Api.Endpoints.Members;


public static class MemberEndpoints
{
    public static IEndpointRouteBuilder MapMemberEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/members", GetMemberSummaries);
        app.MapGet("/members/{id:int}", GetMemberDetails);
        app.MapPost("/members", CreateMember);

        app.MapPut("/members/{id:int}", UpdateMember)
            .RequireAuthorization();

        app.MapDelete("/members/{id:int}", DeleteMember)
            .RequireAuthorization();

        return app;
    }

    public static async Task<IResult> GetMemberSummaries(
       [AsParameters] GetMemberSummariesRequest request,
       GetMemberSummariesQueryHandler query)
    {
        var members = await query.Execute(request);

        return Results.Ok(members);
    }


    public static async Task<IResult> GetMemberDetails(int id, GetMemberDetailsQueryHandler query)
    {
        var member = await query.Execute(id);

        if (member is null)
        {
            return Results.NotFound();
        }
        return Results.Ok(member);
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
         UpdateMemberRequest request,
         ClaimsPrincipal user,
         UpdateMemberCommandHandler handler)
    {
        if (!IsCurrentMember(user, id))
        {
            return Results.Forbid();
        }
        try
        {
            var updated = await handler.Execute(id, request);

            if (!updated)
            {
                return Results.NotFound();
            }
            return Results.NoContent();
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
        ClaimsPrincipal user,
        DeleteMemberCommandHandler handler)
    {
        if (!IsCurrentMember(user, id))
        {
            return Results.Forbid();
        }
        try
        {
            var deleted = await handler.Execute(id);

            if (!deleted)
            {
                return Results.NotFound();
            }

            return Results.NoContent();
        }
        catch (DomainException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }

    }

    private static bool IsCurrentMember(
    ClaimsPrincipal user,
    int memberId)
    {
        var claim =
            user.FindFirstValue(ClaimTypes.NameIdentifier);

        return int.TryParse(claim, out var currentMemberId)
            && currentMemberId == memberId;
    }
}