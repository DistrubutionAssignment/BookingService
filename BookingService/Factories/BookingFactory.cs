using BookingService.DTOs;
using BookingService.Models;

namespace BookingService.Factories;

public static class BookingFactory
{
    public static BookingDto ToDto(BookingModel b) => new BookingDto
    {
        Id = b.Id,
        EventId = b.EventId,
        UserId = b.UserId,
        BookingDate = b.BookingDate
    };
}
