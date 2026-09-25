using BookNook.Data.DTO;
using BookNook.Data.Models;

namespace BookNook.Data.Repository.Interface;

public interface IBookRepository
{
    Task<BookDto> GetByIdAsync(int id);
    Task<IEnumerable<BookDto>> GetAllBooksAsync();
    Task<BookDto> AddBook(BookDto book);
    void Update(BookDto book);
    void Delete(BookDto book);
    Task SaveAsync();
}