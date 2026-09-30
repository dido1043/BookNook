using BookNook.Data.DTO;
using BookNook.Data.Models;
using BookNook.Data.Repository.Interface;
using Microsoft.EntityFrameworkCore;

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

    public Task<IEnumerable<BookDto>> GetFilteredBooksAsync(string? searchQuery, string? genreFilter, bool inStockOnly) =>
        _bookRepository.GetFilteredBooksAsync(searchQuery, genreFilter, inStockOnly);

    public Task<IEnumerable<string>> GetAllGenresAsync() => _bookRepository.GetAllGenresAsync();

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

    public async Task<string?> DeleteBook(BookDto book)
    {
        try
        {
            _bookRepository.Delete(book);
            await _bookRepository.SaveAsync();
            return null;
        }
        catch (DbUpdateException)
        {
            return "ERR: CANNOT_DELETE_BOOK. This book is part of an existing order and is locked by the system.";
        }
    }
}
