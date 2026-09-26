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
    }
}