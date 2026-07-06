/*using BookTracker.Api.Storage;

namespace BookTracker.Api.Application.BookList;

public class GetBookListQuery(IBookRepository bookRepository)
{
    public async Task<IReadOnlyList<BookInfo>> Execute()
    {
        var books = await bookRepository.GetAllAsync();
        var summary = books.Select(book => new BookInfo
        {
            Id = book.Id,
            Title = book.Title.Value,
            Author = book.Author.Value,

        });
        return [.. summary]; //tolist
    }
}
*/

using BookTracker.Api.Storage;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Api.Application.BookList;

public class GetBookListQuery(AppDbContext dbContext)
{
    public async Task<IReadOnlyList<BookInfo>> Execute()
    {
        return await dbContext.Books
            .AsNoTracking()
            .Select(book =>
                new BookInfo
                {
                    Id = book.Id,
                    Title = book.Title.Value,
                    Author = book.Author.Value
                })
            .ToListAsync();
    }
}
