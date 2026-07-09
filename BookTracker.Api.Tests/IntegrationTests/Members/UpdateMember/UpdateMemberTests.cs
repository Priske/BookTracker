using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Members.UpdateMember;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.Members.UpdateMember;

public class UpdateMemberTests : IntegrationTest

{
    [Fact]
    public async Task PutMemberUpdatesMember()
    {
        Writer.Seed(db =>
        {
            db.Members.Add(
                            new Member
                            {
                                Name = new MemberName("Karl"),
                                Email = new MemberEmail("karl@marx.de")
                            });
        });

        var request =
            new UpdateMemberRequest
            {
                Name = "Friedrich",
                Email = "friedrich@engels.de"
            };

        var response = await Client.PutAsJsonAsync("/members/1", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var member = Reader.Query(db => db.Members.Find(1));

        Assert.NotNull(member);
        Assert.Equal("Friedrich", member.Name);
        Assert.Equal("friedrich@engels.de", member.Email);
    }

    [Fact]
    public async Task PutMemberReturnsNotFoundWhenMemberDoesNotExist()
    {
        var request =
            new UpdateMemberRequest
            {
                Name = "Unknown Member",
                Email = "Unknown@email",
            };

        var response = await Client.PutAsJsonAsync("/members/9999", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    [Fact]

    public async Task PutMemberRejectsUpdatedMemberWithExistingEmail()
    {
        Writer.Seed(db =>
        {
            db.Members.AddRange(
                            new Member
                            {
                                Name = new MemberName("Karl"),
                                Email = new MemberEmail("karl@marx.de"),
                                PasswordHash = ""
                            },
                            new Member
                            {
                                Name = new MemberName("Fried"),
                                Email = new MemberEmail("friedrich@engels.de"),
                                PasswordHash = ""
                            }
                            );
        });

        var request =
            new UpdateMemberRequest
            {
                Name = "Friedrich",
                Email = "friedrich@engels.de"
            };

        var response = await Client.PutAsJsonAsync("/members/1", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.Conflict);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

}