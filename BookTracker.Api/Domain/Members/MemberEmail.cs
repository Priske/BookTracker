namespace BookTracker.Api.Domain.Members;

public sealed record MemberEmail
{
    public const int MaxLength = 200;

    public string Value { get; }

    public MemberEmail(string value)
    {
        var cleaned = value?.Trim();

        if (string.IsNullOrWhiteSpace(cleaned))
        {
            throw new DomainException("Email is required.");
        }
        if (!cleaned.Contains('@'))
        {
            throw new DomainException($"Email must contain the @ symbol");
        }
        if (cleaned.Length > MaxLength)
        {
            throw new DomainException($"Email cannot be longer than {MaxLength} characters.");
        }

        Value = cleaned;
    }

    public static implicit operator string(MemberEmail member)
    {
        return member.Value;
    }

    public override string ToString()
    {
        return Value;
    }
}
