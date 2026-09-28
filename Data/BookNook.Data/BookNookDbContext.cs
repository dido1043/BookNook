using BookNook.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookNook.Data
{
    public class BookNookContext : IdentityDbContext<Client, IdentityRole<int>, int>
    {
        public BookNookContext(DbContextOptions<BookNookContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderLine> OrderLines { get; set; } = null!;
        public DbSet<Client> Clients => Users;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {   
            base.OnModelCreating(modelBuilder);
            //Set table name to Clients instead of AspNetUsers
            modelBuilder.Entity<Client>().ToTable("Clients");
            // Cannot delete a Client if they have Orders
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Client)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cannot delete a Book if it has been ordered (OrderLines)
            modelBuilder.Entity<OrderLine>()
                .HasOne(ol => ol.Book)
                .WithMany()
                .HasForeignKey(ol => ol.BookId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}