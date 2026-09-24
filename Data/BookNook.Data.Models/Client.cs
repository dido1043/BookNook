using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BookNook.Data.Models
{
    public class Client : IdentityUser<int>
    {
        [Required]
        [MaxLength(14)]
        public string RecordChannel { get; set; } = string.Empty;
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;
        
        [StringLength(250, ErrorMessage = "Delivery address cannot exceed 250 characters.")]
        public string? DeliveryAddress { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}