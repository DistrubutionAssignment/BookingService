using System.Security.Claims;
using BookingService.Data;
using BookingService.DTOs;
using BookingService.Factories;
using BookingService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookingController : ControllerBase
{
    private readonly DataContext _context;

    public BookingController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings()
    {
        var isAdmin = User.IsInRole("Admin");
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var query = _context.Bookings.AsQueryable();
        if (!isAdmin)
            query = query.Where(b => b.UserId == userId);

        var list = await query.ToListAsync();
        return Ok(list.Select(BookingFactory.ToDto));

    }
    [HttpGet("{id}")]
    public async Task<ActionResult<BookingDto>> GetBooking(string id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null)
            return NotFound();

        var isAdmin = User.IsInRole("Admin");
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!isAdmin && booking.UserId != userId)
            return Forbid();

        return Ok(BookingFactory.ToDto(booking));
    }

    [HttpPost]
    public async Task<ActionResult<BookingDto>> CreateBooking(CreateBookingDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var booking = new BookingModel
        {
            EventId = dto.EventId,
            UserId = userId,
            BookingDate = DateTime.UtcNow
        };
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, BookingFactory.ToDto(booking));
    }


    [HttpPost("{id}")]
    public async Task<IActionResult> DeleteBooking(string id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null) return NotFound();

        var isAdmin = User.IsInRole("Admin");
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!isAdmin && booking.UserId != userId)
            return Forbid();
        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
