using BookNook.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BookNook.Data
{
    public class BookNookContext : DbContext
    {
        public BookNookContext(DbContextOptions<BookNookContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<Client> Clients { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderLine> OrderLines { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Index on Book Title named "idx_title_seek"
            modelBuilder.Entity<Book>()
                .HasIndex(b => b.Title)
                .HasDatabaseName("idx_title_seek");

            // Order numbers must start at 7310
            modelBuilder.Entity<Order>()
                .Property(o => o.Id)
                .UseIdentityColumn(7310, 1);

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