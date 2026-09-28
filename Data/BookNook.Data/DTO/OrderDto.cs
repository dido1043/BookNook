using BookNook.Data.Models;

namespace BookNook.Data.DTO;

public class OrderDto
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public DeliveryMethod DeliveryMethod { get; set; }
    public decimal TotalSum { get; set; }
    public List<OrderLineDto> OrderLines { get; set; } = new();
}
