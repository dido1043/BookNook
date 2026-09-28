using BookNook.Data.DTO;
using BookNook.Data.Models;

namespace BookNook.Web.Models
{
    public class OrderListViewModel
    {
        public IEnumerable<OrderDto> Orders { get; set; } = new List<OrderDto>();
        public string? ClientSearch { get; set; }
        public OrderStatus? StatusFilter { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
