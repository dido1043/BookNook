using BookNook.Data;
using BookNook.Data.DTO;
using BookNook.Data.Models;
using BookNook.Data.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace BookNook.Services.Data.Repository;

public class OrderRepository : IOrderRepository
{
    private readonly BookNookContext _context;
    private readonly Mapper.Mapper _mapper;

    public OrderRepository(BookNookContext context, Mapper.Mapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<OrderDto?> GetByIdAsync(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Client)
            .Include(o => o.OrderLines)
                .ThenInclude(ol => ol.Book)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id);
        if (order == null)
        {
            return null;
        }
        return _mapper.OrderToDto(order);
    }

    public async Task<IEnumerable<OrderDto>> GetAllAsync(
        string? clientSearch,
        OrderStatus? statusFilter,
        DateTime? startDate,
        DateTime? endDate)
    {
        var query = _context.Orders
            .Include(o => o.Client)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(clientSearch))
        {
            var searchLower = clientSearch.ToLower();
            query = query.Where(o => o.Client.Name.ToLower().Contains(searchLower));
        }

        if (statusFilter.HasValue)
        {
            query = query.Where(o => o.Status == statusFilter.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(o => o.OrderDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            var endOfPeriod = endDate.Value.AddDays(1);
            query = query.Where(o => o.OrderDate < endOfPeriod);
        }

        var orders = await query
            .OrderByDescending(o => o.OrderDate)
            .AsNoTracking()
            .ToListAsync();

        return _mapper.ToOrderDtoList(orders);
    }

    public async Task<OrderDto> AddAsync(OrderDto orderDto)
    {
        var entity = _mapper.DtoToOrder(orderDto);
        await _context.Orders.AddAsync(entity);
        await _context.SaveChangesAsync();
        return _mapper.OrderToDto(entity);
    }

    public void Update(OrderDto orderDto)
    {
        _context.Orders.Update(_mapper.DtoToOrder(orderDto));
    }

    public void Delete(OrderDto orderDto)
    {
        _context.Orders.Remove(_mapper.DtoToOrder(orderDto));
    }

    public Task<bool> ExistsAsync(int id)
    {
        return _context.Orders.AnyAsync(o => o.Id == id);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}
