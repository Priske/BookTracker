using BookTracker.Api.Domain.Members;
using BookTracker.Api.Domain;
namespace BookTracker.Api.Tests.IntegrationTests.Members.Domain;

public class MemberNameTests
{
    [Fact]
    public void MemberNamecceptsValidName()
    {
        var member = new MemberName("F. Scott Fitzgerald");

        Assert.Equal("F. Scott Fitzgerald", member.Value);
    }

    [Fact]
    public void AuthorNameTrimsValue()
    {
        var member = new MemberName("   Test");
        Assert.Equal("Test", member.Value);
    }

    [Fact]
    public void MemeberNameRejectsWhitespace()
    {
        var exception = Assert.Throws<DomainException>(() => new MemberName("   "));
        Assert.Equal("Member is required.", exception.Message);
    }

    [Fact]
    public void MemeberNameRejectsNameLongerThan100Characters()
    {
        var exception = Assert.Throws<DomainException>(() => new MemberName(
            "efmuqsqnxtbfadqrkewdnefmksjagaplixrulfeywivafhyocycsgxaqyeedwegwgdcnjrfqzsydnpwbpjuozmsbvsrsqfkzmtmon"));
        Assert.Equal("Member name cannot be longer than 100 characters.", exception.Message);

    }

    [Fact]
    public void MemberNameRejectsNullInput()
    {
        var exception = Assert.Throws<DomainException>(() => new MemberName(null));
        Assert.Equal("Member is required.", exception.Message);
    }

}