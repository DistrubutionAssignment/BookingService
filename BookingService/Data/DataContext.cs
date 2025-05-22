using BookingService.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }

    public DbSet<BookingModel> Bookings { get; set; } = null!;
    public DbSet<EventModel> Events { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookingModel>()
            .HasKey(b => b.Id);

        modelBuilder.Entity<EventModel>()
            .HasKey(e => e.Id);

        base.OnModelCreating(modelBuilder);
    }
}


