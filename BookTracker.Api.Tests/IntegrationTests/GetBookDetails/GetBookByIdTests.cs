using System.Net;
using BookTracker.Api.Application.GetBookDetails;
using BookTracker.Api.Domain;

namespace BookTracker.Api.Tests.IntegrationTests.GetBookDetails;

public class GetBookDetails : IntegrationTest
{
    [Fact]
    public async Task GetBookDetailsReturnsBook()
    {
        Writer.Seed(db =>
        {
            db.Books.Add(
                new Book
                {
                    Title = new BookTitle("Dune"),
                    Author = new AuthorName("Frank Herbert"),
                    Year = 1965
                });
        });

        var response = await Client.GetAsync("/books/1");
        var book = await response.ReadJsonAs<GetBookDetailsResponse>(HttpStatusCode.OK);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(book);
        Assert.Equal(1, book.Id);
        Assert.Equal("Dune", book.Title);
        Assert.Equal("Frank Herbert", book.Author);
        Assert.Equal(1965, book.Year);
    }

    [Fact]
    public async Task GetBookDetailsReturnsNotFoundWhenBookDoesNotExist()
    {
        var response = await Client.GetAsync("/books/9999");
        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
    }
}