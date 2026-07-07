namespace BookTracker.Api.Domain.Members;

public sealed record MemberName
{
    public const int MaxLength = 100;

    public string Value { get; }

    public MemberName(string value)
    {
        var cleaned = value?.Trim();

        if (string.IsNullOrWhiteSpace(cleaned))
        {
            throw new DomainException("Member is required.");
        }

        if (cleaned.Length > MaxLength)
        {
            throw new DomainException($"Member name cannot be longer than {MaxLength} characters.");
        }

        Value = cleaned;
    }

    public static implicit operator string(MemberName member)
    {
        return member.Value;
    }

    public override string ToString()
    {
        return Value;
    }
}
