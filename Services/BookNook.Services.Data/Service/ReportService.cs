using BookNook.Data;
using BookNook.Data.Models;
using BookNook.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BookNook.Services.Data.Service;

public class ReportService
{
    private readonly BookNookContext _context;

    public ReportService(BookNookContext context)
    {
        _context = context;
    }

    public async Task<ReportViewModel> GenerateReportDataAsync(DateTime? startDate, DateTime? endDate)
    {
        var query = _context.OrderLines
            .AsNoTracking()
            .Where(ol => ol.Order.Status == OrderStatus.Confirmed || ol.Order.Status == OrderStatus.Fulfilled);

        if (startDate.HasValue)
        {
            query = query.Where(ol => ol.Order.OrderDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            var endOfPeriod = endDate.Value.AddDays(1);
            query = query.Where(ol => ol.Order.OrderDate < endOfPeriod);
        }

        var genreStats = await query
            .GroupBy(ol => ol.Book.Genre)
            .Select(g => new GenreStat
            {
                Genre = g.Key,
                UnitsSold = g.Sum(ol => ol.Quantity),
                Revenue = g.Sum(ol => ol.Quantity * ol.UnitPrice)
            })
            .OrderByDescending(g => g.Revenue)
            .ToListAsync();

        var topBooks = await query
            .GroupBy(ol => new { ol.Book.Title, ol.Book.Author })
            .Select(g => new TopBookStat
            {
                Title = g.Key.Title,
                Author = g.Key.Author,
                UnitsSold = g.Sum(ol => ol.Quantity)
            })
            .OrderByDescending(b => b.UnitsSold)
            .Take(5)
            .ToListAsync();

        return new ReportViewModel
        {
            StartDate = startDate,
            EndDate = endDate,
            GenreStats = genreStats,
            TopBooks = topBooks
        };
    }
}