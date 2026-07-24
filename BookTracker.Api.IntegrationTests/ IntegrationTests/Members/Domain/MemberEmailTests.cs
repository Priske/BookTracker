using BookTracker.Api.Domain.Members;
using BookTracker.Api.Domain;
namespace BookTracker.Api.IntegrationTests.Members.Domain;

public class MemberEmailTests
{
    [Fact]
    public void MemberNamecceptsValidName()
    {
        var email = new MemberEmail("ScottFitzgerald@email.com");

        Assert.Equal("scottfitzgerald@email.com", email.Value);
    }

    [Fact]
    public void MemberEmailTrimsValue()
    {
        var email = new MemberEmail("   Test@email.com");
        Assert.Equal("test@email.com", email.Value);
    }

    [Fact]
    public void MemberEmailRejectsWhitespace()
    {
        var exception = Assert.Throws<DomainException>(() => new MemberEmail("   "));
        Assert.Equal("Email is required.", exception.Message);
    }

    [Fact]
    public void MemberEmailRejectsNameLongerThan200Characters()
    {
        var exception = Assert.Throws<DomainException>(() => new MemberEmail(
            "efmuqsqnxtbfadqrkewdnefmksjagaplixrulfeywivafhyocycsgxaqyeedwegwgdcn@jrfqzsydnpwbpjuozmsbvsrsqfkzmtmonefmuqsqnxtbfadqrkewdnefmksjagaplixrulfeywivafhyocycsgxaqyeedwegwgdcn@jrfqzsydnpwbpjuozmsbvsrsqfkzmtmon"));
        Assert.Equal("Email cannot be longer than 200 characters.", exception.Message);

    }

    [Fact]
    public void MemberEmailRejectsNullInput()
    {
        var exception = Assert.Throws<DomainException>(() => new MemberEmail(null));
        Assert.Equal("Email is required.", exception.Message);
    }

    [Fact]
    public void MemberEmailRejectInputNotInMailFormat()
    {
        var exception = Assert.Throws<DomainException
        >(() => new MemberEmail("Testemail.com"));
        Assert.Equal("Email must contain the @ symbol", exception.Message);
    }
    [Fact]
    public void MemberEmailToStringReturnsValue()
    {
        var email = new MemberEmail("test@email.com");

        Assert.Equal("test@email.com", email.ToString());
    }
    [Fact]
    public void MemberEmailImplicitlyConvertsToString()
    {
        var email = new MemberEmail("test@email.com");

        string value = email;

        Assert.Equal("test@email.com", value);
    }

    [Fact]
    public void MemberEmailNormalizesValue()
    {
        var email = new MemberEmail("  Ada@Example.com  ");

        Assert.Equal("ada@example.com", email.Value);
    }


}