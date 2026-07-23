using System.Net;
using BookTracker.Api.Application.Members.GetMemberDetails;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.Members.GetMemberDetails;

[Collection(PostgreSqlCollection.Name)]
public class GetMemberDetailsTests(PostgreSqlFixture database)
    : IntegrationTest(database)
{
    [Fact]
    public async Task GetMemberDetailsReturnsBook()
    {
        await AuthenticateAsMember(
            MemberRole.Administrator);
        Writer.Seed(db =>
        {
            db.Members.Add(
                new Member
                {
                    Name = new MemberName("Dune"),
                    Email = new MemberEmail("Frank@Herbert"),
                });
        });

        var response = await Client.GetAsync("/members/2");
        var member = await response.ReadJsonAs<GetMemberDetailsResponse>(HttpStatusCode.OK);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(member);
        Assert.Equal(2, member.Id);
        Assert.Equal("Dune", member.Name);
        Assert.Equal("frank@herbert", member.Email);
    }

    [Fact]
    public async Task GetMemberDetailsReturnsNotFoundWhenMemberDoesNotExist()
    {
        await AuthenticateAsMember(
             MemberRole.Administrator);
        var response = await Client.GetAsync("/members/9999");
        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
    }
}