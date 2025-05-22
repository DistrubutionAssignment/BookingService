namespace BookingService.DTOs;

public class CreateBookingDto
{
    public string EventId { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string City { get; set; } = null!;
    public int TicketAmount { get; set; }
    public string PostalCode { get; set; } = null!;
}

