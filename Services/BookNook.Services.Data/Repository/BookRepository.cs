using BookNook.Data;
using BookNook.Data.DTO;
using BookNook.Data.Models;
using BookNook.Data.Repository.Interface;
using BookNook.Services.Data.Mapper;
using Microsoft.EntityFrameworkCore;

namespace BookNook.Services.Data.Repository;

public class BookRepository : IBookRepository
{
    
    private readonly BookNookContext  _context;
    private readonly Mapper.Mapper _bookMapper;
    
    public BookRepository(BookNookContext  context, Mapper.Mapper bookMapper)
    {
        _context = context;
        _bookMapper = bookMapper;
    }
    
    public async Task<BookDto> GetByIdAsync(int id)
    {
        var book = await _context.Books
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id);
        return _bookMapper.BookToDto(book);
    }

    public Task<IEnumerable<BookDto>> GetAllBooksAsync()
    {
        List<BookDto> books =  _bookMapper.ToBookDtoList(_context.Books.ToList());
        return  Task.FromResult<IEnumerable<BookDto>>(books);
    }

    public async Task<IEnumerable<BookDto>> GetFilteredBooksAsync(string? searchQuery, string? genreFilter, bool inStockOnly)
    {
        var query = _context.Books.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            var searchLower = searchQuery.ToLower();
            query = query.Where(b => b.Title.ToLower().Contains(searchLower) || b.Author.ToLower().Contains(searchLower));
        }

        if (!string.IsNullOrWhiteSpace(genreFilter))
            query = query.Where(b => b.Genre == genreFilter);

        if (inStockOnly)
            query = query.Where(b => b.AvailableQuantity > 0);

        var books = await query.ToListAsync();
        return _bookMapper.ToBookDtoList(books);
    }

    public async Task<IEnumerable<string>> GetAllGenresAsync()
    {
        return await _context.Books.Select(b => b.Genre).Distinct().ToListAsync();
    }

    public async Task<BookDto> AddBook(BookDto book)                                                                                                                                                                                      
    {                                                                                                                                                                                                                                      
        var entity = _bookMapper.DtoToBook(book);                                                                                                                                                                                          
        await _context.Books.AddAsync(entity);                                                                                                                                                                                             
        await _context.SaveChangesAsync();                                                                                                                                                                                                 
        return _bookMapper.BookToDto(entity);                                                                                                                                                                                              
    }     

    public void Update(BookDto book)
    {
        _context.Books.Update(_bookMapper.DtoToBook(book));
    }

    public void Delete(BookDto book)
    {
        _context.Books.Remove(_bookMapper.DtoToBook(book));
    }

    public Task SaveAsync()
    {
        _context.SaveChanges();
        return Task.CompletedTask;
    }
}