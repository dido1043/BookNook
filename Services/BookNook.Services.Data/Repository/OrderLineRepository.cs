using BookNook.Data;
using BookNook.Data.DTO;
using BookNook.Data.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace BookNook.Services.Data.Repository;

public class OrderLineRepository : IOrderLineRepository
{
    private readonly BookNookContext _context;
    private readonly Mapper.Mapper _mapper;

    public OrderLineRepository(BookNookContext context, Mapper.Mapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<OrderLineDto?> GetByIdAsync(int orderId, int bookId)
    {
        var orderLine = await _context.OrderLines
            .AsNoTracking()
            .FirstOrDefaultAsync(ol => ol.OrderId == orderId && ol.BookId == bookId);
        return orderLine is null ? null : _mapper.OLToDto(orderLine);
    }

    public async Task<IEnumerable<OrderLineDto>> GetAllAsync()
    {
        var orderLines = await _context.OrderLines
            .AsNoTracking()
            .ToListAsync();
        return orderLines.Select(_mapper.OLToDto).ToList();
    }

    public async Task<IEnumerable<OrderLineDto>> GetByOrderIdAsync(int orderId)
    {
        var orderLines = await _context.OrderLines
            .AsNoTracking()
            .Where(ol => ol.OrderId == orderId)
            .ToListAsync();
        return orderLines.Select(_mapper.OLToDto).ToList();
    }

    public async Task<OrderLineDto> AddAsync(OrderLineDto orderLine)
    {
        var entity = _mapper.DtoToOL(orderLine);
        await _context.OrderLines.AddAsync(entity);
        await _context.SaveChangesAsync();
        return _mapper.OLToDto(entity);
    }

    public void Update(OrderLineDto orderLine)
    {
        _context.OrderLines.Update(_mapper.DtoToOL(orderLine));
    }

    public void Delete(OrderLineDto orderLine)
    {
        _context.OrderLines.Remove(_mapper.DtoToOL(orderLine));
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}
