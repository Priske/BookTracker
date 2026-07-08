using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Members.CreateMember;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.Members.CreateMember;

public class CreatMemberTests : IntegrationTest
{

    [Fact]
    public async Task PostMemberCreatesBook()
    {
        var request =
            new CreateMemberRequest
            {
                Name = "For Petes Sake",
                Email = "PP@PePe.com",
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
        Assert.Equal("PP@PePe.com", member.Email);
        Assert.Equal("For Petes Sake", member.Name);
    }
}