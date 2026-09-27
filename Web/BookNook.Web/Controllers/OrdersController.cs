using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookNook.Data;
using BookNook.Data.Models;
using BookNook.Web.Models;

namespace BookNook.Web.Controllers
{
    public class OrdersController : Controller
    {
        private readonly BookNookContext _context;
        private readonly IConfiguration _configuration;

        public OrdersController(BookNookContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(string? clientSearch, OrderStatus? statusFilter, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Orders.Include(o => o.Client).AsQueryable();

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

            query = query.OrderByDescending(o => o.OrderDate);

            var viewModel = new OrderListViewModel
            {
                Orders = await query.ToListAsync(),
                ClientSearch = clientSearch,
                StatusFilter = statusFilter,
                StartDate = startDate,
                EndDate = endDate
            };

            return View(viewModel);
        }

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

            var order = new Order
            {
                ClientId = viewModel.ClientId,
                OrderDate = DateTime.Now,
                Status = OrderStatus.New,
                DeliveryMethod = viewModel.DeliveryMethod,
                OrderLines = new List<OrderLine>()
            };

            decimal itemsTotal = 0;

            foreach (var lineVm in viewModel.OrderLines)
            {
                var book = await _context.Books.FindAsync(lineVm.BookId);

                if (book == null) continue;

                if (lineVm.Quantity > book.AvailableQuantity)
                {
                    ModelState.AddModelError(string.Empty, $"Insufficient stock for {book.Title}. Max available: {book.AvailableQuantity}");
                    return View(viewModel);
                }

                order.OrderLines.Add(new OrderLine
                {
                    BookId = book.Id,
                    Quantity = lineVm.Quantity,
                    UnitPrice = book.Price
                });

                itemsTotal += (book.Price * lineVm.Quantity);
            }

            var storeSettings = _configuration.GetSection("StoreSettings");
            decimal discountThreshold = storeSettings.GetValue<decimal>("DiscountThreshold");
            decimal discountPercent = storeSettings.GetValue<decimal>("DiscountPercentage");
            decimal deliveryPrice = storeSettings.GetValue<decimal>("DeliveryPrice");

            decimal discountAmount = 0;
            decimal deliveryFee = 0;

            if (itemsTotal > discountThreshold)
            {
                discountAmount = itemsTotal * discountPercent;
            }

            if (order.DeliveryMethod == DeliveryMethod.Delivery && discountAmount == 0)
            {
                deliveryFee = deliveryPrice;
            }

            order.TotalSum = itemsTotal - discountAmount + deliveryFee;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders
                .Include(o => o.Client)
                .Include(o => o.OrderLines)
                    .ThenInclude(ol => ol.Book)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null) return NotFound();

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrderStatus newStatus)
        {
            var order = await _context.Orders
                .Include(o => o.OrderLines)
                    .ThenInclude(ol => ol.Book)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null) return NotFound();

            if (order.Status == OrderStatus.Fulfilled)
            {
                ModelState.AddModelError(string.Empty, "Cannot modify a fulfilled order.");
                return View(order);
            }

            if (order.Status == newStatus)
            {
                return RedirectToAction(nameof(Index));
            }

            var oldStatus = order.Status;
            order.Status = newStatus;

            foreach (var line in order.OrderLines)
            {
                if (newStatus == OrderStatus.Confirmed && oldStatus != OrderStatus.Confirmed)
                {
                    if (line.Book.AvailableQuantity < line.Quantity)
                    {
                        ModelState.AddModelError(string.Empty, $"Insufficient stock for '{line.Book.Title}'. Cannot confirm.");
                        order.Status = oldStatus;
                        return View(order);
                    }
                    line.Book.AvailableQuantity -= line.Quantity;
                }

                else if (oldStatus == OrderStatus.Confirmed && (newStatus == OrderStatus.Rejected || newStatus == OrderStatus.New))
                {
                    line.Book.AvailableQuantity += line.Quantity;
                }
            }

            try
            {
                _context.Update(order);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(order.Id)) return NotFound();
                else throw;
            }

            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}