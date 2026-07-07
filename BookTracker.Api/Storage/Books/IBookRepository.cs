using BookTracker.Api.Domain.Books;

namespace BookTracker.Api.Storage.Books;

public interface IBookRepository
{
    Task<Book> AddAsync(Book book);
    Task<bool> DeleteAsync(int id);
    Task<bool> UpdateAsync(Book book);
}