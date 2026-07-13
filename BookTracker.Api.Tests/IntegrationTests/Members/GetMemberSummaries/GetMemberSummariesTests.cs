using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Members.GetMemberSummaries;
using BookTracker.Api.Domain.Members;


namespace BookTracker.Api.Tests.IntegrationTests.Members.GetMemberSummaries;

public class GetMemberSummariesTests : IntegrationTest
{

    [Fact]
    public async Task GetMemberSummaries()
    {
        await AuthenticateAsMember(MemberRole.Administrator);
        Writer.Seed(db => db.Members.Add(
                new Member
                {
                    Name = new MemberName("Cannery Row"),
                    Email = new MemberEmail("John@Steinbeck"),
                    PasswordHash = ""
                }
            ));


        var response = await Client.GetAsync("/members");
        var result = await response.ReadJsonAs<GetMemberSummariesResponse>(HttpStatusCode.OK);

        Assert.NotNull(result);

        var memberSummary = Assert.Single(
                result.Items,
                member => member.Email == "john@steinbeck");
        Assert.Equal("john@steinbeck", memberSummary.Email);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(2, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public async Task GetMemberSummariesRejectsAsMembers()
    {
        await AuthenticateAsMember(MemberRole.Member);
        Writer.Seed(db => db.Members.Add(
                new Member
                {
                    Name = new MemberName("Cannery Row"),
                    Email = new MemberEmail("John@Steinbeck"),
                    PasswordHash = ""
                }
            ));

        var response = await Client.GetAsync("/members");
        await response.ShouldHaveStatusCode(HttpStatusCode.Forbidden);

    }

    [Fact]
    public async Task GetMemberSummariesReturnsRequestedPage()
    {
        await AuthenticateAsMember(MemberRole.Administrator);
        Writer.Seed(db =>
        {
            db.Members.AddRange(
                new Member
                {
                    Name = new MemberName("Jefke"),
                    Email = new MemberEmail("Jefke@jef.jef")
                },
                new Member
                {
                    Name = new MemberName("Jefke 2"),
                    Email = new MemberEmail("Jefke2@jef.jef")
                },
                new Member
                {
                    Name = new MemberName("Jefke 3"),
                    Email = new MemberEmail("Jefke3@jef.jef")

                });
        });

        var result = await Client.GetFromJsonAsync<GetMemberSummariesResponse>("/members?page=2&pageSize=1");

        Assert.NotNull(result);

        var member = Assert.Single(result.Items);

        Assert.Equal("Jefke", result.Items[0].Name);
        Assert.Equal(2, result.Page);
        Assert.Equal(1, result.PageSize);
        Assert.Equal(4, result.TotalItems);
        Assert.Equal(4, result.TotalPages);
    }

    [Fact]
    public async Task GetMemberSummariesReturnsEmptyItemsWhenPageIsTooHigh()
    {
        await AuthenticateAsMember(
            MemberRole.Administrator);
        Writer.Seed(db =>
        {
            db.Members.Add(
                new Member
                {
                    Name = new MemberName("Jefke 3"),
                    Email = new MemberEmail("Jefke3@jef.jef")
                });
        });

        var result = await Client.GetFromJsonAsync<GetMemberSummariesResponse>("/members?page=99&pageSize=10");

        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(99, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(2, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public async Task GetMemberSummariesCanSearchByEmail()
    {
        await AuthenticateAsMember(MemberRole.Administrator);
        Writer.Seed(db =>
        {
            db.Members.AddRange(
                new Member
                {
                    Name = new MemberName("Jefke 3"),
                    Email = new MemberEmail("Jefke3@jef.jef")
                },
                new Member
                {
                    Name = new MemberName("Karl"),
                    Email = new MemberEmail("Karl@Marx.de")
                });
        });

        var response = await Client.GetAsync("/members?search=Marx");

        var result = await response.ReadJsonAs<GetMemberSummariesResponse>(HttpStatusCode.OK);

        var member = Assert.Single(result.Items);

        Assert.Equal("Karl", member.Name);
        Assert.Equal("karl@marx.de", member.Email);
        Assert.Equal(1, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
    }


    [Fact]
    public async Task GetMemberSummariesCanSearchByName()
    {
        await AuthenticateAsMember(MemberRole.Administrator);
        Writer.Seed(db =>
        {
            db.Members.AddRange(
                new Member
                {
                    Name = new MemberName("Jefke 3"),
                    Email = new MemberEmail("Jefke3@jef.jef")
                },
                new Member
                {
                    Name = new MemberName("Karl"),
                    Email = new MemberEmail("Karl@Marx.de")
                });
        });

        var response = await Client.GetAsync("/members?search=Karl");

        var result = await response.ReadJsonAs<GetMemberSummariesResponse>(HttpStatusCode.OK);

        var member = Assert.Single(result.Items);

        Assert.Equal("Karl", member.Name);
        Assert.Equal("karl@marx.de", member.Email);
        Assert.Equal(1, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
    }


    [Fact]
    public async Task GetMemberSummariesAppliesPagingAfterSearch()
    {
        await AuthenticateAsMember(MemberRole.Administrator);
        Writer.Seed(db =>
        {
            db.Members.AddRange(
            new Member
            {
                Name = new MemberName("Karl"),
                Email = new MemberEmail("karl@marx.de")
            },
            new Member
            {
                Name = new MemberName("Friedrich"),
                Email = new MemberEmail("friedrich@engels.de")
            },
            new Member
            {
                Name = new MemberName("Jane"),
                Email = new MemberEmail("jane.austen@email.com")
            },
            new Member
            {
                Name = new MemberName("George"),
                Email = new MemberEmail("george.orwell@email.com")
            });
        });

        var response = await Client.GetAsync("/members?search=email&page=2&pageSize=1");

        var result = await response.ReadJsonAs<GetMemberSummariesResponse>(HttpStatusCode.OK);

        var member = Assert.Single(result.Items);

        Assert.Equal("george.orwell@email.com", member.Email);
        Assert.Equal(2, result.TotalItems);
        Assert.Equal(2, result.TotalPages);
        Assert.Equal(2, result.Page);
        Assert.Equal(1, result.PageSize);
    }

    [Fact]
    public async Task GetMemberSummariesSearchForNoResuls()
    {
        await AuthenticateAsMember(MemberRole.Administrator);
        Writer.Seed(db =>
            {
                db.Members.AddRange(
                new Member
                {
                    Name = new MemberName("Karl"),
                    Email = new MemberEmail("karl@marx.de")
                },
                new Member
                {
                    Name = new MemberName("Friedrich"),
                    Email = new MemberEmail("friedrich@engels.de")
                },
                new Member
                {
                    Name = new MemberName("Jane"),
                    Email = new MemberEmail("jane.austen@email.com")
                },
                new Member
                {
                    Name = new MemberName("George"),
                    Email = new MemberEmail("george.orwell@email.com")
                });
            });
        var response = await Client.GetAsync("/members?search=Commits&page=2&pageSize=1");

        var result = await response.ReadJsonAs<GetMemberSummariesResponse>(HttpStatusCode.OK);

        Assert.Empty(result.Items);

        Assert.Equal(2, result.Page);
        Assert.Equal(1, result.PageSize);
        Assert.Equal(0, result.TotalItems);
        Assert.Equal(0, result.TotalPages);
    }
}

