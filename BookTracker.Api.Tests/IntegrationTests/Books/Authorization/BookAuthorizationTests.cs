using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.Books.CreateBook;
using BookTracker.Api.Application.Books.UpdateBook;
using BookTracker.Api.Domain.Books;
using BookTracker.Api.Domain.Members;

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

    [Fact]
    public async Task RegularMemberCannotCreateBook()
    {
        await AuthenticateAsMember();

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
            HttpStatusCode.Forbidden);

        var count =
            Reader.Query(db =>
                db.Books.Count());

        Assert.Equal(0, count);
    }

    [Fact]
    public async Task RegularMemberCannotUpdateBook()
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

        await AuthenticateAsMember();

        var request =
            new UpdateBookRequest
            {
                Title = "Dunes",
                Author = "Frank Herbert Updated",
                Year = 1966
            };

        var response =
            await Client.PutAsJsonAsync(
                "/books/1",
                request);

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Forbidden);

        var book =
            Reader.Query(db =>
                db.Books.Single());

        Assert.Equal("Dune", book.Title.Value);
        Assert.Equal("Frank Herbert", book.Author.Value);
        Assert.Equal(1965, book.Year);

    }

    [Fact]
    public async Task RegularMemberCannotDeleteBook()
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

        await AuthenticateAsMember();

        var response = await Client.DeleteAsync("/books/1");

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Forbidden);

        var book =
            Reader.Query(db =>
                db.Books.Single());

        Assert.Equal("Dune", book.Title.Value);
        Assert.Equal("Frank Herbert", book.Author.Value);
        Assert.Equal(1965, book.Year);

    }


    [Fact]
    public async Task AdministratorCanManageBooks()
    {
        await AuthenticateAsMember(MemberRole.Administrator);

        var createRequest =
            new CreateBookRequest
            {
                Title = "Dune",
                Author = "Frank Herbert",
                Year = 1965
            };

        var createResponse =
            await Client.PostAsJsonAsync(
                "/books",
                createRequest);

        await createResponse.ShouldHaveStatusCode(
            HttpStatusCode.Created);

        var updateRequest =
            new UpdateBookRequest
            {
                Title = "Dune",
                Author = "Frank Herbert",
                Year = 1966
            };

        var updateResponse =
                    await Client.PutAsJsonAsync(
                        "/books/1",
                        updateRequest);

        await updateResponse.ShouldHaveStatusCode(
                    HttpStatusCode.NoContent);

        var response = await Client.DeleteAsync("/books/1");

        await response.ShouldHaveStatusCode(
            HttpStatusCode.NoContent);

    }


}
