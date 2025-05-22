namespace BookingService.Models;

public class EventModel //minimalistiskt eventmodel eftersom jag använder en gemensam databas men behöver sätta Id och pris på biljetter för själva bokningen. 
{
    public string Id { get; set; } = null!;
    public decimal Price { get; set; }
}
