using System.Net;

using BookTracker.Api.IntegrationTests;

namespace BookTracker.Tests.IntegrationTests.Members.DeleteMember;


[Collection(PostgreSqlCollection.Name)]
public class DeleteMemberTests(PostgreSqlFixture database)
    : IntegrationTest(database)
{
    [Fact]
    public async Task DeleteMemberRemovesMember()
    {

        var memberId = await AuthenticateAsMember();
        var response = await Client.DeleteAsync($"/members/{memberId}");

        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var member = Reader.Query(db => db.Books.Find(1));

        Assert.Null(member);
    }

    [Fact]
    public async Task DeleteMemberReturnsUnauthorizedDeletingNonLoggedInMember()
    {
        var response = await Client.DeleteAsync("/members/9999");
        await response.ShouldHaveStatusCode(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteMemberReturnsForbiddenDeletingNonLoggedInMember()
    {
        await AuthenticateAsMember();
        var response = await Client.DeleteAsync("/members/9999");
        await response.ShouldHaveStatusCode(HttpStatusCode.Forbidden);
    }

}