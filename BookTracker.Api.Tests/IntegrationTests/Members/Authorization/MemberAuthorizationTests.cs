using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BookTracker.Api.Application.Auth.Login;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.Members.Authorization;

public class MemberAuthorizationTests : IntegrationTest
{

    [Fact]
    public async Task MemberListRequiresAuthentication()
    {
        var response =
            await Client.GetAsync("/members");

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RegularMemberCannotViewMemberList()
    {
        await AuthenticateAsMember();

        var response =
            await Client.GetAsync("/members");

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AdministratorCanViewMemberList()
    {
        await AuthenticateAsMember(
            MemberRole.Administrator);

        var response =
            await Client.GetAsync("/members");

        await response.ShouldHaveStatusCode(
            HttpStatusCode.OK);
    }


    [Fact]
    public async Task MemberDetailstRequiresAuthentication()
    {
        var response =
            await Client.GetAsync("/members/1");

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Unauthorized);
    }


    [Fact]
    public async Task AdministratorCanViewMemberDetailst()
    {
        await AuthenticateAsMember(
            MemberRole.Administrator);

        var response =
            await Client.GetAsync("/members/1");

        await response.ShouldHaveStatusCode(
            HttpStatusCode.OK);
    }

    [Fact]
    public async Task RegularMemberCannotViewMemberDetails()
    {
        await AuthenticateAsMember();

        var response =
            await Client.GetAsync("/members/1");

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdatedRoleMemberRequiredANewLogin()
    {
        await AuthenticateAsMember();

        var response =
            await Client.GetAsync("/members/1");

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Forbidden);

        Writer.Seed(db =>
            {
                var member = db.Members.Single(member => member.Id == 1);
                member.Role = MemberRole.Administrator;
            });

        response = await Client.GetAsync("/members/1");
        await response.ShouldHaveStatusCode(
       HttpStatusCode.Forbidden);

        var loginResponse = await Client.PostAsJsonAsync(
            "/auth/login",
            new LoginRequest
            {
                Email = "ada@example.com",
                Password = "analytical-engine"
            });

        var login = await loginResponse.ReadJsonAs<LoginResponse>(
        HttpStatusCode.OK);

        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                login.AccessToken);

        response = await Client.GetAsync("/members/1");
        await response.ShouldHaveStatusCode(
       HttpStatusCode.OK);

    }



}