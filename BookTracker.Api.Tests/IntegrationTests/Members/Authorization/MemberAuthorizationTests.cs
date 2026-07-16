using System.Net;
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


}