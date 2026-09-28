using BookNook.Data.DTO;
using BookNook.Data.Repository.Interface;

namespace BookNook.Services.Data.Service;

public class OrderLineService
{
    private readonly IOrderLineRepository _orderLineRepository;

    public OrderLineService(IOrderLineRepository orderLineRepository)
    {
        _orderLineRepository = orderLineRepository;
    }

    public async Task<OrderLineDto?> GetByIdAsync(int orderId, int bookId)
    {
        return await _orderLineRepository.GetByIdAsync(orderId, bookId);
    }

    public async Task<IEnumerable<OrderLineDto>> AllOrderLines()
    {
        return await _orderLineRepository.GetAllAsync();
    }

    public async Task<IEnumerable<OrderLineDto>> GetByOrderIdAsync(int orderId)
    {
        return await _orderLineRepository.GetByOrderIdAsync(orderId);
    }

    public async Task<OrderLineDto> AddOrderLine(OrderLineDto orderLine)
    {
        var created = await _orderLineRepository.AddAsync(orderLine);
        await _orderLineRepository.SaveAsync();
        return created;
    }

    public async Task UpdateOrderLine(OrderLineDto orderLine)
    {
        _orderLineRepository.Update(orderLine);
        await _orderLineRepository.SaveAsync();
    }

    public async Task DeleteOrderLine(OrderLineDto orderLine)
    {
        _orderLineRepository.Delete(orderLine);
        await _orderLineRepository.SaveAsync();
    }
}
