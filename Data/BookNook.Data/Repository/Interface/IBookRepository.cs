using BookNook.Data.DTO;
using BookNook.Data.Models;

namespace BookNook.Data.Repository.Interface;

public interface IBookRepository
{
    Task<BookDto> GetByIdAsync(int id);
    Task<IEnumerable<BookDto>> GetAllBooksAsync();
    Task<IEnumerable<BookDto>> GetFilteredBooksAsync(string? searchQuery, string? genreFilter, bool inStockOnly);
    Task<IEnumerable<string>> GetAllGenresAsync();
    Task<BookDto> AddBook(BookDto book);
    void Update(BookDto book);
    void Delete(BookDto book);
    Task SaveAsync();
}