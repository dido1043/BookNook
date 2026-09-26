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

        public OrdersController(BookNookContext context)
        {
            _context = context;
        }

        // GET: Orders/Create?bookId=5
        public async Task<IActionResult> Create(int? bookId)
        {
            var viewModel = new OrderFormViewModel
            {
                Status = OrderStatus.New,
                AvailableClients = await _context.Clients.OrderBy(c => c.Name).ToListAsync(),
                AvailableBooks = await _context.Books.Where(b => b.AvailableQuantity > 0).OrderBy(b => b.Title).ToListAsync()
            };

            // if add to order is clicked pre-populate that book
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
    }
}