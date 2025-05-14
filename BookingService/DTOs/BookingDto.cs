namespace BookingService.DTOs;

public class BookingDto
{
    public string Id { get; set; } = null!;
    public string EventId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public DateTime BookingDate { get; set; }
}

