using System.ComponentModel.DataAnnotations;

namespace BookNook.Data.DTO;

public class BookDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Author is required.")]
    public string Author { get; set; } = string.Empty;

    [Required(ErrorMessage = "Genre is required.")]
    public string Genre { get; set; } = string.Empty;

    [Range(0, 10000, ErrorMessage = "Quantity cannot be negative.")]
    public int AvailableQuantity { get; set; }

    [Range(0.01, 1000.00, ErrorMessage = "Price must be a positive number.")]
    public decimal Price { get; set; }
}