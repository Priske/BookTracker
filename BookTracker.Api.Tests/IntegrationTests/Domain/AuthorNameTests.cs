using BookTracker.Api.Domain;

namespace BookTracker.Api.Tests.IntegrationTests.Domain;

public class AuthorNameTests
{
    [Fact]
    public void AuthorNameAcceptsValidName()
    {
        var author = new AuthorName("F. Scott Fitzgerald");

        Assert.Equal("F. Scott Fitzgerald", author.Value);
    }

    [Fact]
    public void AuthorNameTrimsValue()
    {
        var author = new AuthorName("   Test");
        Assert.Equal("Test", author.Value);
    }

    [Fact]
    public void AuthorNameRejectsWhitespace()
    {
        var exception = Assert.Throws<DomainException>(() => new AuthorName("   "));
        Assert.Equal("Author is required.", exception.Message);
    }

    [Fact]
    public void AuthorNameRejectsNameLongerThan100Characters()
    {
        var exception = Assert.Throws<DomainException>(() => new AuthorName(
            "efmuqsqnxtbfadqrkewdnefmksjagaplixrulfeywivafhyocycsgxaqyeedwegwgdcnjrfqzsydnpwbpjuozmsbvsrsqfkzmtmon"));
        Assert.Equal("Author cannot be longer than 100 characters.", exception.Message);

    }

    [Fact]
    public void AuthorNameRejectsNullInput()
    {
        var exception = Assert.Throws<DomainException>(() => new AuthorName(null));
        Assert.Equal("Author is required.", exception.Message);
    }

}