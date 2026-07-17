using BookTracker.Api.Domain.Members;
using Microsoft.AspNetCore.Identity;
using QuickFuzzr;

namespace BookTracker.Api.Seeding;

public static class MemberFuzzr
{
    private static readonly PasswordHasher<Member> Hasher = new();

    private static Member CreateMember(string name, string email, string password)
    {
        var member = new Member
        {
            Name = new MemberName(name),
            Email = new MemberEmail(email)
        };

        member.PasswordHash = Hasher.HashPassword(member, password);

        return member;
    }
    public static IEnumerable<Member> Many(int count)
    {
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        var members = new List<Member>(count);
        var usedEmails = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase);

        while (members.Count < count)
        {
            var member = One.Generate();

            // Replace `.Value` if MemberEmail exposes its value differently.
            if (usedEmails.Add(member.Email.Value))
            {
                members.Add(member);
            }
        }

        return members;
    }

    private static readonly string[] Adjectives =
    [
        "Suspicious",
        "Melancholy",
        "Quantum",
        "Reluctant",
        "Extremely Polite",
        "Mildly Haunted",
        "Unreasonably Confident",
        "Invisible",
        "Chronically Late",
        "Over-Caffeinated"
    ];

    private static readonly string[] Nouns =
    [
        "Badger",
        "Librarian",
        "Spaceship",
        "Cupcake",
        "Philosopher",
        "Typewriter",
        "Goblin",
        "Umbrella",
        "Database",
        "Octopus"
    ];

    private static readonly string[] FirstNames =
    [
        "Ada",
        "Grace",
        "Douglas",
        "Ursula",
        "Terry",
        "Octavia",
        "Isaac",
        "Mary",
        "Kurt",
        "Agatha"
    ];

    private static readonly string[] LastNames =
    [
        "Byte",
        "Stackwell",
        "Nullman",
        "Loopington",
        "Brackets",
        "Mergefield",
        "Bugworthy",
        "Semicolon",
        "Heap",
        "Async"
    ];

    private static readonly FuzzrOf<string> Gmail =
        from adjective in Fuzzr.OneOf(Adjectives)
        from noun in Fuzzr.OneOf(Nouns)
        from firstname in Fuzzr.OneOf(FirstNames)
        from lastname in Fuzzr.OneOf(LastNames)
        select $"{firstname}{adjective}{noun}{lastname}@Gmail.com";

    private static readonly FuzzrOf<string> Hotmail =
        from adjective in Fuzzr.OneOf(Adjectives)
        from noun in Fuzzr.OneOf(Nouns)
        from firstname in Fuzzr.OneOf(FirstNames)
        from lastname in Fuzzr.OneOf(LastNames)
        select $"{firstname}{adjective}{lastname}{noun}@Hotmail.com";

    private static readonly FuzzrOf<string> Pswrds =
           from adjective in Fuzzr.OneOf(Adjectives)
           from noun in Fuzzr.OneOf(Nouns)
           select $"{adjective}{noun}";

    private static readonly FuzzrOf<string> Email =
        Fuzzr.OneOf(Gmail, Hotmail);

    private static readonly FuzzrOf<string> Password =
        Fuzzr.OneOf(Pswrds);
    private static readonly FuzzrOf<string> Name =
        from firstName in Fuzzr.OneOf(FirstNames)
        from lastName in Fuzzr.OneOf(LastNames)
        select $"{firstName}{lastName}";

    private static readonly FuzzrOf<Member> One =
        from name in Name
        from email in Email
        from password in Password
        select CreateMember(name, email, password);
}

