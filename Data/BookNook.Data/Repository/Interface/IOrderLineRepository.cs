using BookNook.Data.DTO;

namespace BookNook.Data.Repository.Interface;

public interface IOrderLineRepository
{
    Task<OrderLineDto?> GetByIdAsync(int orderId, int bookId);
    Task<IEnumerable<OrderLineDto>> GetAllAsync();
    Task<IEnumerable<OrderLineDto>> GetByOrderIdAsync(int orderId);
    Task<OrderLineDto> AddAsync(OrderLineDto orderLine);
    void Update(OrderLineDto orderLine);
    void Delete(OrderLineDto orderLine);
    Task SaveAsync();
}
