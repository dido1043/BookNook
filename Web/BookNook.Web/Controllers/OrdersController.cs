using BookNook.Data;
using BookNook.Data.Models;
using BookNook.Services.Data.Service;
using BookNook.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

        public async Task<IActionResult> Index(string? clientSearch, OrderStatus? statusFilter, DateTime? startDate, DateTime? endDate)
        {
            var orders = await _orderService.GetAllAsync(clientSearch, statusFilter, startDate, endDate);

            bool isAdmin = User.IsInRole("Admin");
            if (!isAdmin)
            {
                int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

                orders = orders.Where(o => o.ClientId == currentUserId).ToList();

                clientSearch = null;
            }

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

        [HttpGet]
        public async Task<IActionResult> Create(int? bookId)
        {
            var viewModel = new OrderFormViewModel
            {
                Status = OrderStatus.New,
                AvailableBooks = await _context.Books.Where(b => b.AvailableQuantity > 0).OrderBy(b => b.Title).ToListAsync()
            };

            bool isAdmin = User.IsInRole("Admin");
            int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            if (isAdmin)
            {
                viewModel.AvailableClients = await _context.Clients.OrderBy(c => c.Name).ToListAsync();
            }
            else
            {
                viewModel.AvailableClients = await _context.Clients.Where(c => c.Id == currentUserId).ToListAsync();
                viewModel.ClientId = currentUserId;
            }

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
        public async Task<IActionResult> Create(OrderFormViewModel viewModel)
        {
            bool isAdmin = User.IsInRole("Admin");
            int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            if (isAdmin)
            {
                viewModel.AvailableClients = await _context.Clients.OrderBy(c => c.Name).ToListAsync();
            }
            else
            {
                viewModel.ClientId = currentUserId;
                viewModel.AvailableClients = await _context.Clients.Where(c => c.Id == currentUserId).ToListAsync();
            }

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
