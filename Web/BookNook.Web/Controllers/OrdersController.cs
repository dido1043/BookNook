using BookNook.Data;
using BookNook.Data.Models;
using BookNook.Services.Data.Service;
using BookNook.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookNook.Web.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly OrderService _orderService;
        private readonly BookNookContext _context;

        public OrdersController(OrderService orderService, BookNookContext context)
        {
            _orderService = orderService;
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string? clientSearch, OrderStatus? statusFilter, DateTime? startDate, DateTime? endDate)
        {
            var orders = await _orderService.GetAllAsync(clientSearch, statusFilter, startDate, endDate);

            var viewModel = new OrderListViewModel
            {
                Orders = orders,
                ClientSearch = clientSearch,
                StatusFilter = statusFilter,
                StartDate = startDate,
                EndDate = endDate
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Create(int? bookId)
        {
            var viewModel = new OrderFormViewModel
            {
                Status = OrderStatus.New,
                AvailableClients = await _context.Clients.OrderBy(c => c.Name).ToListAsync(),
                AvailableBooks = await _context.Books.Where(b => b.AvailableQuantity > 0).OrderBy(b => b.Title).ToListAsync()
            };

            if (bookId.HasValue)
            {
                var book = await _context.Books.FindAsync(bookId.Value);
                if (book != null && book.AvailableQuantity > 0)
                {
                    viewModel.OrderLines.Add(new OrderLineViewModel
                    {
                        BookId = book.Id,
                        BookTitle = book.Title,
                        UnitPrice = book.Price,
                        Quantity = 1,
                        MaxQuantity = book.AvailableQuantity
                    });
                }
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Create(OrderFormViewModel viewModel)
        {
            viewModel.AvailableClients = await _context.Clients.OrderBy(c => c.Name).ToListAsync();
            viewModel.AvailableBooks = await _context.Books.Where(b => b.AvailableQuantity > 0).OrderBy(b => b.Title).ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            if (viewModel.OrderLines == null || !viewModel.OrderLines.Any())
            {
                ModelState.AddModelError(string.Empty, "An order must contain at least one book.");
                return View(viewModel);
            }

            var error = await _orderService.CreateOrderAsync(
                viewModel.ClientId,
                viewModel.DeliveryMethod,
                viewModel.OrderLines.Select(l => (l.BookId, l.Quantity)));

            if (error != null)
            {
                ModelState.AddModelError(string.Empty, error);
                return View(viewModel);
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var order = await _orderService.GetByIdAsync(id.Value);
            if (order == null) return NotFound();

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, OrderStatus newStatus)
        {
            var error = await _orderService.UpdateStatusAsync(id, newStatus);

            if (error != null)
            {
                if (!await _orderService.ExistsAsync(id)) return NotFound();

                var order = await _orderService.GetByIdAsync(id);
                ModelState.AddModelError(string.Empty, error);
                return View(order);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
