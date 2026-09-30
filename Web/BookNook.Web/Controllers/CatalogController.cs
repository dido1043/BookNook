using Microsoft.AspNetCore.Mvc;
using BookNook.Services.Data.Service;
using BookNook.Web.Models;

namespace BookNook.Web.Controllers
{
    public class CatalogController : Controller
    {
        private readonly BookService _bookService;

        public CatalogController(BookService bookService)
        {
            _bookService = bookService;
        }

        public async Task<IActionResult> Index(string? searchQuery, string? genreFilter, bool inStockOnly = false)
        {
            var vm = new CatalogViewModel
            {
                Books = await _bookService.GetFilteredBooksAsync(searchQuery, genreFilter, inStockOnly),
                AvailableGenres = await _bookService.GetAllGenresAsync(),
                SearchQuery = searchQuery,
                GenreFilter = genreFilter,
                InStockOnly = inStockOnly
            };

            return View(vm);
        }
    }
}