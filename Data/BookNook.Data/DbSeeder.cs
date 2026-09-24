using BookNook.Data.Models;

namespace BookNook.Data
{
    public static class DbSeeder
    {
        public static void Seed(BookNookContext context)
        {
            if (context.Books.Any())
            {
                return;
            }

            var books = new List<Book>
            {
                new Book { Title = "The Pragmatic Programmer", Author = "David Thomas", Genre = "Programming", Price = 45.00m, AvailableQuantity = 10 },
                new Book { Title = "Clean Code", Author = "Robert C. Martin", Genre = "Programming", Price = 40.00m, AvailableQuantity = 5 },
                new Book { Title = "Dune", Author = "Frank Herbert", Genre = "Sci-Fi", Price = 25.00m, AvailableQuantity = 20 },
                new Book { Title = "Foundation", Author = "Isaac Asimov", Genre = "Sci-Fi", Price = 20.00m, AvailableQuantity = 15 },
                new Book { Title = "The Hobbit", Author = "J.R.R. Tolkien", Genre = "Fantasy", Price = 15.50m, AvailableQuantity = 8 }
            };
            context.Books.AddRange(books);
            context.SaveChanges();

            var client = new Client
            {
                Name = "Ivan Ivanov",
                Phone = "0888123456",
                Email = "ivan@example.com",
                DeliveryAddress = "Plovdiv, ul. Gladston 1"
            };
            context.Clients.Add(client);
            context.SaveChanges();

            var order = new Order
            {
                ClientId = client.Id,
                OrderDate = DateTime.Now.AddDays(-2),
                Status = OrderStatus.New,
                DeliveryMethod = DeliveryMethod.Delivery,
                TotalSum = 65.00m
            };
            context.Orders.Add(order);
            context.SaveChanges();

            var orderLines = new List<OrderLine>
            {
                new OrderLine { OrderId = order.Id, BookId = books[0].Id, Quantity = 1, UnitPrice = 45.00m },
                new OrderLine { OrderId = order.Id, BookId = books[3].Id, Quantity = 1, UnitPrice = 20.00m }
            };
            context.OrderLines.AddRange(orderLines);
            context.SaveChanges();
        }
    }
}