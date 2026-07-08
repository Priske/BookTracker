using System.Net;
using BookTracker.Api.Application.Members.GetMemberDetails;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.Members.GetMemberDetails;

public class GetMemberDetails : IntegrationTest
{
    [Fact]
    public async Task GetMemberDetailsReturnsBook()
    {
        Writer.Seed(db =>
        {
            db.Members.Add(
                new Member
                {
                    Name = new MemberName("Dune"),
                    Email = new MemberEmail("Frank@Herbert"),
                });
        });

        var response = await Client.GetAsync("/members/1");
        var member = await response.ReadJsonAs<GetMemberDetailsResponse>(HttpStatusCode.OK);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(member);
        Assert.Equal(1, member.Id);
        Assert.Equal("Dune", member.Name);
        Assert.Equal("Frank@Herbert", member.Email);
    }

    [Fact]
    public async Task GetMemberDetailsReturnsNotFoundWhenMemberDoesNotExist()
    {
        var response = await Client.GetAsync("/members/9999");
        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
    }
}