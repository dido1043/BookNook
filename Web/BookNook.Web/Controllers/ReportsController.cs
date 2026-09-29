using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookNook.Data;
using BookNook.Data.Models;
using BookNook.Web.Models;
using System.Text;

namespace BookNook.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly BookNookContext _context;

        public ReportsController(BookNookContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            var viewModel = await GenerateReportDataAsync(startDate, endDate);
            return View(viewModel);
        }

        public async Task<IActionResult> ExportCsv(DateTime? startDate, DateTime? endDate)
        {
            var data = await GenerateReportDataAsync(startDate, endDate);

            var builder = new StringBuilder();
            builder.AppendLine("--- GENRE STATISTICS ---");
            builder.AppendLine("Genre,Units Sold,Revenue (EUR)");

            foreach (var stat in data.GenreStats)
            {
                builder.AppendLine($"{stat.Genre},{stat.UnitsSold},{stat.Revenue:F2}");
            }

            builder.AppendLine();
            builder.AppendLine("--- TOP 5 BEST-SELLING BOOKS ---");
            builder.AppendLine("Title,Author,Units Sold");

            foreach (var book in data.TopBooks)
            {
                var safeTitle = book.Title.Replace(",", "");
                var safeAuthor = book.Author.Replace(",", "");
                builder.AppendLine($"{safeTitle},{safeAuthor},{book.UnitsSold}");
            }

            var fileName = $"BookNook_Report_{DateTime.Now:yyyyMMdd}.csv";
            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", fileName);
        }

        private async Task<ReportViewModel> GenerateReportDataAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.OrderLines
                .Include(ol => ol.Order)
                .Include(ol => ol.Book)
                .Where(ol => ol.Order.Status == OrderStatus.Confirmed || ol.Order.Status == OrderStatus.Fulfilled)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(ol => ol.Order.OrderDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                var endOfPeriod = endDate.Value.AddDays(1);
                query = query.Where(ol => ol.Order.OrderDate < endOfPeriod);
            }

            var allValidLines = await query.ToListAsync();

            var genreStats = allValidLines
                .GroupBy(ol => ol.Book.Genre)
                .Select(g => new GenreStat
                {
                    Genre = g.Key,
                    UnitsSold = g.Sum(ol => ol.Quantity),
                    Revenue = g.Sum(ol => ol.Quantity * ol.UnitPrice)
                })
                .OrderByDescending(g => g.Revenue)
                .ToList();

            var topBooks = allValidLines
                .GroupBy(ol => new { ol.Book.Title, ol.Book.Author })
                .Select(g => new TopBookStat
                {
                    Title = g.Key.Title,
                    Author = g.Key.Author,
                    UnitsSold = g.Sum(ol => ol.Quantity)
                })
                .OrderByDescending(b => b.UnitsSold)
                .Take(5)
                .ToList();

            return new ReportViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                GenreStats = genreStats,
                TopBooks = topBooks
            };
        }
    }
}