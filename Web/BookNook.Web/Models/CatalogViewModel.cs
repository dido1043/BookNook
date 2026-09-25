using BookNook.Data.Models;

namespace BookNook.Web.Models
{
    public class CatalogViewModel
    {
        public IEnumerable<Book> Books { get; set; } = new List<Book>();
        public string? SearchQuery { get; set; }
        public string? GenreFilter { get; set; }
        public bool InStockOnly { get; set; }
        public IEnumerable<string> AvailableGenres { get; set; } = new List<string>();
    }
}