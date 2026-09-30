using BookNook.Data.DTO;

namespace BookNook.Web.Models
{
    public class CatalogViewModel
    {
        public IEnumerable<BookDto> Books { get; set; } = new List<BookDto>();
        public string? SearchQuery { get; set; }
        public string? GenreFilter { get; set; }
        public bool InStockOnly { get; set; }
        public IEnumerable<string> AvailableGenres { get; set; } = new List<string>();
    }
}