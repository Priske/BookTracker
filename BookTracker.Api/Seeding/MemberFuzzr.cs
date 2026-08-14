using BookTracker.Api.Domain.Members;
using Microsoft.AspNetCore.Identity;
using QuickFuzzr;

namespace BookTracker.Api.Seeding;

public static class MemberFuzzr
{
    public static IEnumerable<Member> Many(int count)
        => One.Many(count).Generate();

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

    private static readonly FuzzrOf<string> Suffix =
        Fuzzr.String(Fuzzr.Char('0', '9'), 3).Unique("email-suffix");

    private static readonly FuzzrOf<string> Password =
           from adjective in Fuzzr.OneOf(DataLists.Adjectives)
           from noun in Fuzzr.OneOf(DataLists.Nouns)
           select $"{adjective}{noun}";

    private static readonly FuzzrOf<Member> One =
        from firstName in Fuzzr.OneOf(DataLists.FirstNames)
        from lastName in Fuzzr.OneOf(DataLists.LastNames)
        let name = $"{firstName} {lastName}"
        from suffix in Suffix
        from emailProvider in Fuzzr.OneOf(DataLists.EmailProviders)
        let email = $"{firstName}.{lastName}_{suffix}@{emailProvider}".ToLower()
        from password in Password
        select CreateMember(name, email, password);
}

