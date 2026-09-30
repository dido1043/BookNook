using BookNook.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace BookNook.Data
{
    public static class DbSeeder
    {
        public const string AdminRole = "Admin";
        public const string ClientRole = "Client";

        public static async Task SeedAsync(
            BookNookContext context,
            RoleManager<IdentityRole<int>> roleManager,
            UserManager<Client> userManager)
        {
            await SeedRolesAsync(roleManager);
            await SeedAdminAsync(userManager);

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
                UserName = "ivan@example.com",
                DeliveryAddress = "Plovdiv, ul. Gladston 1"
            };
            var createResult = await userManager.CreateAsync(client, "Password123!");
            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(client, ClientRole);
            }

            var client2 = new Client
            {
                Name = "Maria Petrova",
                Phone = "0899123123",
                Email = "maria@example.com",
                UserName = "maria@example.com",
                DeliveryAddress = "Sofia, bul. Vitosha 42"
            };
            var createResult2 = await userManager.CreateAsync(client2, "Password123!");
            if (createResult2.Succeeded)
            {
                await userManager.AddToRoleAsync(client2, ClientRole);
            }

            var client3 = new Client
            {
                Name = "Georgi Georgiev",
                Phone = "0877987654",
                Email = "georgi@example.com",
                UserName = "georgi@example.com",
                DeliveryAddress = null
            };
            var createResult3 = await userManager.CreateAsync(client3, "Password123!");
            if (createResult3.Succeeded)
            {
                await userManager.AddToRoleAsync(client3, ClientRole);
            }

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

            var order2 = new Order
            {
                ClientId = client2.Id,
                OrderDate = DateTime.Now.AddDays(-5),
                Status = OrderStatus.Confirmed,
                DeliveryMethod = DeliveryMethod.InStore,
                TotalSum = 40.00m
            };
            var order3 = new Order
            {
                ClientId = client3.Id,
                OrderDate = DateTime.Now.AddDays(-10),
                Status = OrderStatus.Fulfilled,
                DeliveryMethod = DeliveryMethod.Delivery,
                TotalSum = 45.50m
            };
            var order4 = new Order
            {
                ClientId = client.Id,
                OrderDate = DateTime.Now.AddDays(-1),
                Status = OrderStatus.Rejected,
                DeliveryMethod = DeliveryMethod.Delivery,
                TotalSum = 25.00m
            };

            context.Orders.AddRange(order2, order3, order4);
            context.SaveChanges();

            var extraOrderLines = new List<OrderLine>
            {
                new OrderLine { OrderId = order2.Id, BookId = books[1].Id, Quantity = 1, UnitPrice = 40.00m },
                
                new OrderLine { OrderId = order3.Id, BookId = books[4].Id, Quantity = 1, UnitPrice = 15.50m },
                new OrderLine { OrderId = order3.Id, BookId = books[2].Id, Quantity = 1, UnitPrice = 25.00m },
                
                new OrderLine { OrderId = order4.Id, BookId = books[3].Id, Quantity = 1, UnitPrice = 20.00m }
            };
            context.OrderLines.AddRange(extraOrderLines);
            context.SaveChanges();
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole<int>> roleManager)
        {
            foreach (var role in new[] { AdminRole, ClientRole })
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(role));
                }
            }
        }

        private static async Task SeedAdminAsync(UserManager<Client> userManager)
        {
            const string adminEmail = "admin@booknook.local";
            if (await userManager.FindByEmailAsync(adminEmail) != null) return;

            var admin = new Client
            {
                Name = "System Admin",
                Phone = "0000000000",
                Email = adminEmail,
                UserName = adminEmail,
                DeliveryAddress = null
            };
            var result = await userManager.CreateAsync(admin, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, AdminRole);
            }
        }
    }
}