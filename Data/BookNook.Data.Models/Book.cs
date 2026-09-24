using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookNook.Data.Models
{
    public class Book : BaseEntity
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 200 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Author is required.")]
        [StringLength(100, ErrorMessage = "Author name cannot exceed 100 characters.")]
        public string Author { get; set; } = string.Empty;

        [Required(ErrorMessage = "Genre is required.")]
        [StringLength(50)]
        public string Genre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a price.")]
        [Range(0.01, 1000.00, ErrorMessage = "Price must be a positive number.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, 10000, ErrorMessage = "Available quantity cannot be negative.")]
        public int AvailableQuantity { get; set; }
    }
}