using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Books.CreateBook;
using BookTracker.Api.Domain.Books;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.Books.CreateBook;

public class CreateBookTests : IntegrationTest
{

    [Fact]
    public async Task PostBookCreatesBook()
    {
        await AuthenticateAsMember(MemberRole.Administrator);
        var request =
            new CreateBookRequest
            {
                Title = "The Heart Is a Lonely Hunter",
                Author = "Carson McCullers",
                Year = 1940
            };
        var response = await Client.PostAsJsonAsync("/books", request);
        var created = await response.ReadJsonAs<CreateBookResponse>(HttpStatusCode.Created);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(created);
        Assert.True(created.Id > 0);
        Assert.Equal("The Heart Is a Lonely Hunter", created.Title);
        //
        // Reader Usage Test
        //
        var book = Reader.Query(context => context.Find<Book>(created.Id));

        Assert.NotNull(book);
        Assert.Equal("The Heart Is a Lonely Hunter", book.Title.Value);
        Assert.Equal("Carson McCullers", book.Author.Value);
        Assert.Equal(1940, book.Year);
    }
}