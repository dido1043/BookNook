using System.ComponentModel.DataAnnotations;
using BookNook.Data.Models;

namespace BookNook.Web.Models
{
    public class OrderFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a client.")]
        [Display(Name = "Client")]
        public int ClientId { get; set; }

        [Required]
        [Display(Name = "Delivery Method")]
        public DeliveryMethod DeliveryMethod { get; set; }

        [Required]
        public OrderStatus Status { get; set; }

        public List<OrderLineViewModel> OrderLines { get; set; } = new List<OrderLineViewModel>();

        public IEnumerable<Client> AvailableClients { get; set; } = new List<Client>();
        public IEnumerable<Book> AvailableBooks { get; set; } = new List<Book>();
    }

    public class OrderLineViewModel
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int MaxQuantity { get; set; }
    }
}