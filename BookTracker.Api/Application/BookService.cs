using BookTracker.Api.Application.BookList;
using BookTracker.Api.Application.CreateBook;
using BookTracker.Api.Domain;
using BookTracker.Api.Storage;

namespace BookTracker.Api.Application;

public class BookService(IBookRepository bookRepository)
{
    public async Task<IReadOnlyList<BookInfo>> GetAllBooks()
    {
        var books = await bookRepository.GetAllAsync();
        var summary = books.Select(book => new BookInfo
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,

        });
        return [.. summary];
    }

    public async Task<CreateBookResponse> CreateBook(CreateBookRequest request)
    {
        var book =
            new Book
            {
                Author = request.Author,
                Title = request.Title,
                Year = request.Year,
                // map de velden van request naar de properties van dit nieuwe object
            };
        var savedBook = await bookRepository.AddAsync(book);
        return
            new CreateBookResponse
            {
                Id = savedBook.Id,
                Title = savedBook.Title,
                Author = savedBook.Author,
                Year = savedBook.Year

                // map de velden van de `savedBook` entiteit naar de properties van de response DTO
            };
    }
}
