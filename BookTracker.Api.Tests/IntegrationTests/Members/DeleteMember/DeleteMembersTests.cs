using System.Net;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.Members.DeleteMember;

public class DeleteMemberTests : IntegrationTest
{
    [Fact]
    public async Task DeleteMemberRemovesMember()
    {
        Writer.Seed(db =>
        {
            db.Members.Add(
                new Member
                {
                    Id = 1,
                    Name = new MemberName("Dune"),
                    Email = new MemberEmail("Frank@Herbert"),
                });
        });

        var response = await Client.DeleteAsync("/members/1");

        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var member = Reader.Query(db => db.Books.Find(1));

        Assert.Null(member);
    }

    [Fact]
    public async Task DeleteMemberReturnsNotFoundWhenMemberDoesNotExist()
    {
        var response = await Client.DeleteAsync("/members/9999");
        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
    }
}