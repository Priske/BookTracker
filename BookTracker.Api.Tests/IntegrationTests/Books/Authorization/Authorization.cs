using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Books.CreateBook;
using BookTracker.Api.Domain.Books;

namespace BookTracker.Api.Tests.IntegrationTests.Books.Authorization;

public class BookAuthorizationTests : IntegrationTest
{

    [Fact]
    public async Task CreateBookRequiresAuthentication()
    {
        var request =
            new CreateBookRequest
            {
                Title = "Dune",
                Author = "Frank Herbert",
                Year = 1965
            };

        var response =
            await Client.PostAsJsonAsync(
                "/books",
                request);

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Unauthorized);

        var count =
            Reader.Query(db => db.Books.Count());

        Assert.Equal(0, count);
    }
}
