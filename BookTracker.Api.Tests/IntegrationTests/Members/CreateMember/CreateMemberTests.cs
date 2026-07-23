using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Members.CreateMember;
using BookTracker.Api.Domain.Members;
using Microsoft.AspNetCore.Identity;

namespace BookTracker.Api.Tests.IntegrationTests.Members.CreateMember;


[Collection(PostgreSqlCollection.Name)]
public class CreateMemberTests(PostgreSqlFixture database)
    : IntegrationTest(database)
{

    [Fact]
    public async Task PostMemberCreatesMember()
    {
        var request =
            new CreateMemberRequest
            {
                Name = "For Petes Sake",
                Email = "PP@PePe.com",
                Password = "adminadmin"
            };
        var response = await Client.PostAsJsonAsync("/members", request);
        var created = await response.ReadJsonAs<CreateMemberResponse>(HttpStatusCode.Created);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(created);
        Assert.True(created.Id > 0);
        Assert.Equal("For Petes Sake", created.Name);
        //
        // Reader Usage Test
        //
        var member = Reader.Query(context => context.Find<Member>(created.Id));

        Assert.NotNull(member);
        Assert.Equal("pp@pepe.com", member.Email);
        Assert.Equal("For Petes Sake", member.Name);
    }


    [Fact]
    public async Task PostMemberWhitespaceReturnsBadRequest()
    {
        var request = new CreateMemberRequest
        {
            Name = "    ",
            Email = "PP@PePe.com",
            Password = "adminadmin"
        };

        var response = await Client.PostAsJsonAsync("/members", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostMemberInvalidEmailReturnsBadRequest()
    {
        var request = new CreateMemberRequest
        {
            Name = "For Petes Sake",
            Email = "PPPePe.com",
            Password = "adminadmin"
        };

        var response = await Client.PostAsJsonAsync("/members", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task MemberEmailRejectsDuplicateEmail()
    {

        var request = new CreateMemberRequest
        {
            Name = "For Petes Sake",
            Email = "PPP@ePe.com",
            Password = "adminadmin"
        };

        var response1 = await Client.PostAsJsonAsync("/members", request);

        var request2 = new CreateMemberRequest
        {
            Name = "For Petes Sake",
            Email = "PPP@ePe.com",
            Password = "adminadmin"
        };

        var response2 = await Client.PostAsJsonAsync("/members", request2);

        // First request should succeed
        Assert.Equal(HttpStatusCode.Created, response1.StatusCode);

        // Second request should fail due to duplicate email
        Assert.Equal(HttpStatusCode.Conflict, response2.StatusCode);
    }


    [Fact]
    public async Task MemberPasswordGetsHashed()
    {
        var request =
            new CreateMemberRequest
            {
                Name = "For Petes Sake",
                Email = "PP@PePe.com",
                Password = "adminadmin"
            };

        var response = await Client.PostAsJsonAsync("/members", request);
        var created = await response.ReadJsonAs<CreateMemberResponse>(HttpStatusCode.Created);
        var member = Reader.Query(db =>
            db.Members.Single(current => current.Id == created.Id));
        Assert.NotEqual("analytical-engine", member.PasswordHash);

        var passwordHasher = new PasswordHasher<Member>();

        var result = passwordHasher.VerifyHashedPassword(
            member,
            member.PasswordHash,
            "adminadmin");

        Assert.Equal(PasswordVerificationResult.Success, result);

    }

    [Fact]
    public async Task MemberPasswordRejectsEmtpy()
    {
        var request =
            new CreateMemberRequest
            {
                Name = "For Petes Sake",
                Email = "PP@PePe.com",
                Password = ""
            };
        var response = await Client.PostAsJsonAsync("/members", request);


        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }


    [Fact]
    public async Task MemberPasswordRejectsTooShort()
    {
        var request =
            new CreateMemberRequest
            {
                Name = "For Petes Sake",
                Email = "PP@PePe.com",
                Password = "1234"
            };
        var response = await Client.PostAsJsonAsync("/members", request);


        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateMemberCreatesRegularMember()
    {
        var request =
            new CreateMemberRequest
            {
                Name = "Grace Hopper",
                Email = "grace@example.com",
                Password = "debugging-moth"
            };

        var response =
            await Client.PostAsJsonAsync(
                "/members",
                request);

        var created =
            await response
                .ReadJsonAs<CreateMemberResponse>(
                    HttpStatusCode.Created);

        var member =
            Reader.Query(db =>
                db.Members.Find(created.Id));

        Assert.NotNull(member);

        Assert.Equal(
            MemberRole.Member,
            member.Role);
    }
}