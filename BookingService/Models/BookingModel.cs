namespace BookingService.Models;

public class BookingModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EventId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
}
