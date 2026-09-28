using BookNook.Data.DTO;
using BookNook.Data.Models;

namespace BookNook.Data.Repository.Interface;

public interface IOrderRepository
{
    Task<OrderDto?> GetByIdAsync(int id);
    Task<IEnumerable<OrderDto>> GetAllAsync(
        string? clientSearch,
        OrderStatus? statusFilter,
        DateTime? startDate,
        DateTime? endDate);
    Task<OrderDto> AddAsync(OrderDto order);
    void Update(OrderDto order);
    void Delete(OrderDto order);
    Task<bool> ExistsAsync(int id);
    Task SaveAsync();
}
