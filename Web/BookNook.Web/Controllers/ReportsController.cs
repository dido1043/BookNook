using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookNook.Services.Data.Service;
using System.Text;

namespace BookNook.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly ReportService _reportService;

        public ReportsController(ReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            var viewModel = await _reportService.GenerateReportDataAsync(startDate, endDate);
            return View(viewModel);
        }

        public async Task<IActionResult> ExportCsv(DateTime? startDate, DateTime? endDate)
        {
            var data = await _reportService.GenerateReportDataAsync(startDate, endDate);

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
    }
}