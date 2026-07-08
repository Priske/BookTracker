using BookTracker.Api.Domain.Books;
using BookTracker.Api.Domain;

namespace BookTracker.Api.Tests.IntegrationTests.Books.Domain;

public class BookTitleTests
{
    [Fact]
    public void BookTitleAcceptsValidTitle()
    {
        var title = new BookTitle("Dune");
        Assert.Equal("Dune", title.Value);
    }

    [Fact]
    public void BookTitleTrimsValue()
    {
        var title = new BookTitle("  Dune  ");
        Assert.Equal("Dune", title.Value);
    }

    [Fact]
    public void BookTitleRejectsWhitespace()
    {
        var exception = Assert.Throws<DomainException>(() => new BookTitle("   "));
        Assert.Equal("Title is required.", exception.Message);
    }

    [Fact]
    public void BookTitleRejectsTitleLongerThan100Characters()
    {
        var tooLong = new string('x', 101);
        var exception = Assert.Throws<DomainException>(() => new BookTitle(tooLong));

        Assert.Equal("Title cannot be longer than 100 characters.", exception.Message);
    }

    [Fact]
    public void BookTitleRejectsNullInput()
    {
        var exception = Assert.Throws<DomainException>(() => new BookTitle(null));
        Assert.Equal("Title is required.", exception.Message);
    }
    [Fact]
    public void BookTitleToStringReturnsValue()
    {
        var title = new BookTitle("The Hobbit");

        Assert.Equal("The Hobbit", title.ToString());
    }

    [Fact]
    public void BookTitleImplicitlyConvertsToString()
    {
        var title = new BookTitle("The Hobbit");

        string value = title;

        Assert.Equal("The Hobbit", value);
    }

}