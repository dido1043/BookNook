using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookNook.Data;
using BookNook.Web.Models;

namespace BookNook.Web.Controllers
{
    public class CatalogController : Controller
    {
        private readonly BookNookContext _context;

        public CatalogController(BookNookContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? searchQuery, string? genreFilter, bool inStockOnly = false)
        {
            var query = _context.Books.AsQueryable();

            // Search by Title OR Author
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var searchLower = searchQuery.ToLower();
                query = query.Where(b => b.Title.ToLower().Contains(searchLower) ||
                                         b.Author.ToLower().Contains(searchLower));
            }

            // Filter by Genre
            if (!string.IsNullOrWhiteSpace(genreFilter))
            {
                query = query.Where(b => b.Genre == genreFilter);
            }

            // Filter by Availability
            if (inStockOnly)
            {
                query = query.Where(b => b.AvailableQuantity > 0);
            }

            var vm = new CatalogViewModel
            {
                Books = await query.ToListAsync(),
                SearchQuery = searchQuery,
                GenreFilter = genreFilter,
                InStockOnly = inStockOnly,
                AvailableGenres = await _context.Books.Select(b => b.Genre).Distinct().ToListAsync()
            };

            return View(vm);
        }
    }
}