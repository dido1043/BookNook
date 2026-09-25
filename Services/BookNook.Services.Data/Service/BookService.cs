using BookNook.Data.DTO;
using BookNook.Data.Models;
using BookNook.Data.Repository.Interface;

namespace BookNook.Services.Data.Service;

public class BookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<BookDto> GetBookById(int id)
    {
        return await _bookRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<BookDto>> AllBooks()
    {
        return await _bookRepository.GetAllBooksAsync();
    }

    public async Task<BookDto> AddBook(BookDto book)
    {
        var created = await _bookRepository.AddBook(book);
        await _bookRepository.SaveAsync();
        return created;
    }

    public async Task UpdateBook(BookDto book)
    {
        _bookRepository.Update(book);
        await _bookRepository.SaveAsync();
    }

    public async Task DeleteBook(BookDto book)
    {
        _bookRepository.Delete(book);
        await _bookRepository.SaveAsync();
    }
}
