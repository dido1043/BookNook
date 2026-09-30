using BookNook.Data.DTO;

namespace BookNook.Web.Models
{
    public class ProfileViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? DeliveryAddress { get; set; }
        public string Role { get; set; } = "Client";
        public IEnumerable<OrderDto> Orders { get; set; } = new List<OrderDto>();
    }
}
