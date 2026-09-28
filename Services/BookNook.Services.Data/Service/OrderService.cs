using BookNook.Data;
using BookNook.Data.DTO;
using BookNook.Data.Models;
using BookNook.Data.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BookNook.Services.Data.Service;

public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly BookNookContext _context;
    private readonly IConfiguration _configuration;

    public OrderService(IOrderRepository orderRepository, BookNookContext context, IConfiguration configuration)
    {
        _orderRepository = orderRepository;
        _context = context;
        _configuration = configuration;
    }

    public Task<OrderDto?> GetByIdAsync(int id) => _orderRepository.GetByIdAsync(id);

    public Task<IEnumerable<OrderDto>> GetAllAsync(string? clientSearch, OrderStatus? statusFilter,
        DateTime? startDate, DateTime? endDate)
    {
        
        return _orderRepository.GetAllAsync(clientSearch, statusFilter, startDate, endDate);
    }

    public Task<bool> ExistsAsync(int id)
    {
        return _orderRepository.ExistsAsync(id);
    }

    public async Task<string?> CreateOrderAsync(int clientId, DeliveryMethod deliveryMethod,
        IEnumerable<(int BookId, int Quantity)> lineInputs)
    {
        var order = new Order
        {
            ClientId = clientId,
            OrderDate = DateTime.Now,
            Status = OrderStatus.New,
            DeliveryMethod = deliveryMethod,
            OrderLines = new List<OrderLine>()
        };

        decimal itemsTotal = 0;

        foreach (var input in lineInputs)
        {
            var book = await _context.Books.FindAsync(input.BookId);
            if (book == null)
            {
                continue;
            }

            if (input.Quantity > book.AvailableQuantity)
            {
                return $"Insufficient stock for {book.Title}. Max available: {book.AvailableQuantity}";
            }

            order.OrderLines.Add(new OrderLine
            {
                BookId = book.Id,
                Quantity = input.Quantity,
                UnitPrice = book.Price
            });

            itemsTotal += book.Price * input.Quantity;
        }

        var storeSettings = _configuration.GetSection("StoreSettings");
        decimal discountThreshold = storeSettings.GetValue<decimal>("DiscountThreshold");
        decimal discountPercent = storeSettings.GetValue<decimal>("DiscountPercentage");
        decimal deliveryPrice = storeSettings.GetValue<decimal>("DeliveryPrice");

        decimal discountAmount = itemsTotal > discountThreshold ? itemsTotal * discountPercent : 0;
        decimal deliveryFee = (order.DeliveryMethod == DeliveryMethod.Delivery && discountAmount == 0)
            ? deliveryPrice
            : 0;

        order.TotalSum = itemsTotal - discountAmount + deliveryFee;

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return "Added order successfully!";
    }

    public async Task<string?> UpdateStatusAsync(int id, OrderStatus newStatus)
    {
        var order = await _context.Orders
            .Include(o => o.OrderLines)
                .ThenInclude(ol => ol.Book)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return "Order not found.";
        }
            
        if (order.Status == OrderStatus.Fulfilled)
        {
            return "Cannot modify a fulfilled order.";
        }
        if (order.Status == newStatus)
        {
            return null;
        }

        var oldStatus = order.Status;
        order.Status = newStatus;

        foreach (var line in order.OrderLines)
        {
            if (newStatus == OrderStatus.Confirmed && oldStatus != OrderStatus.Confirmed)
            {
                if (line.Book.AvailableQuantity < line.Quantity)
                {
                    order.Status = oldStatus;
                    return $"Insufficient stock for '{line.Book.Title}'. Cannot confirm.";
                }
                line.Book.AvailableQuantity -= line.Quantity;
            }
            else if (oldStatus == OrderStatus.Confirmed &&
                     (newStatus == OrderStatus.Rejected || newStatus == OrderStatus.New))
            {
                line.Book.AvailableQuantity += line.Quantity;
            }
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _orderRepository.ExistsAsync(id)) return "Order not found.";
            throw;
        }

        return "Order updated successfully!";
    }
}
