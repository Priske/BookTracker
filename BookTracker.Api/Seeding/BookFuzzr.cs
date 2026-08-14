using BookTracker.Api.Domain.Books;
using QuickFuzzr;

namespace BookTracker.Api.Seeding;

public static class BookFuzzr
{
    public static IEnumerable<Book> Many(int count)
        => One.Many(count).Generate();

    private static readonly FuzzrOf<string> Situational =
        from adjective in Fuzzr.OneOf(DataLists.Adjectives)
        from noun in Fuzzr.OneOf(DataLists.Nouns)
        from situation in Fuzzr.OneOf(DataLists.Situations)
        select $"The {adjective} {noun} {situation}";

    private static readonly FuzzrOf<string> Memoir =
        from adjective in Fuzzr.OneOf(DataLists.Adjectives)
        from noun in Fuzzr.OneOf(DataLists.Nouns)
        select $"My Life as an {adjective} {noun}";

    private static readonly FuzzrOf<string> Academic =
        from adjective in Fuzzr.OneOf(DataLists.Adjectives)
        from noun in Fuzzr.OneOf(DataLists.Nouns)
        select $"A Brief History of {adjective} {noun}s";

    private static readonly FuzzrOf<string> Title =
        Fuzzr.OneOf(Situational, Memoir, Academic);

    private static readonly FuzzrOf<string> Author =
        from firstName in Fuzzr.OneOf(DataLists.FirstNames)
        from lastName in Fuzzr.OneOf(DataLists.LastNames)
        select $"{firstName} {lastName}";

    private static readonly FuzzrOf<Book> One =
        from title in Title
        from author in Author
        from year in Fuzzr.Int(1930, 2026)
        select new Book
        {
            Title = new BookTitle(title),
            Author = new AuthorName(author),
            Year = year
        };
}