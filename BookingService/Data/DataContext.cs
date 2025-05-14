using BookingService.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<BookingModel> Bookings { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<BookingModel>()
                .HasKey(b => b.Id);
        }
    }
}

