namespace BookingService.Models;

public class BookingModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EventId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
    public int TicketAmount {get; set; } = 1;

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string City { get; set; } = null!;
    public string PostalCode { get; set; } = null!;

    public decimal TotalPrice { get; set; }
}
