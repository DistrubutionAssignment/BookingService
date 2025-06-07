using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BookingService.Data;
using BookingService.DTOs;
using BookingService.Factories;
using BookingService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Controllers;

[Authorize] // Endast inloggade användare kan komma åt dessa endpoints
[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly DataContext _context;

    public BookingController(DataContext context)
    {
        _context = context;
    }
    // GET 
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings()
    {
        try {
            var isAdmin = User.IsInRole("Admin");
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            var q = _context.Bookings.AsQueryable();
            if (!isAdmin)
                q = q.Where(b => b.UserId == userId);

            var list = await q.ToListAsync();
            return Ok(list.Select(BookingFactory.ToDto));
        }
        catch (Exception ex)
        {
        Console.Error.WriteLine(ex);
        return StatusCode(500, ex.Message);
        }

    }

    // GET 
    [HttpGet("{id}")]
    public async Task<ActionResult<BookingDto>> GetBooking(string id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null) return NotFound();

        var isAdmin = User.IsInRole("Admin");
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        if (!isAdmin && booking.UserId != userId)
            return Forbid();

        return Ok(BookingFactory.ToDto(booking));
    }

    // POST 
    [HttpPost]
    public async Task<ActionResult<BookingDto>> CreateBooking(CreateBookingDto dto)
    {
        //Hämta event-pris från samma DB
        var evt = await _context.Events.FindAsync(dto.EventId);
        if (evt == null)
            return BadRequest("Event not found");

        //Beräkna totalpris (antal biljetter finns i dto)
        var total = evt.Price * dto.TicketAmount;

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        if (userId == null)
            return Unauthorized();


        // Skapa booking
        var booking = new BookingModel
        {
            EventId = dto.EventId,
            UserId = userId,
            BookingDate = DateTime.UtcNow,
            TicketAmount = dto.TicketAmount,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Address = dto.Address,
            City = dto.City,
            PostalCode = dto.PostalCode,
            TotalPrice = total
        };
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        //. Returnera skapad booking
        return CreatedAtAction(
            nameof(GetBooking),
            new { id = booking.Id },
            BookingFactory.ToDto(booking)
        );
    }

    // DELETE 
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")] //endast admin kan ta bort bokningar
    public async Task<IActionResult> DeleteBooking(string id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null) return NotFound();

        var isAdmin = User.IsInRole("Admin");
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        if (!isAdmin && booking.UserId != userId)
            return Forbid();

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
