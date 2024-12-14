using System;
using BookingApplication.Core.Entities;
using BookingApplication.Core.Repositories;
using BookingApplication.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BookingApplication.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly BookingContext _context;

    public BookingRepository(BookingContext context)
    {
        _context = context;
    }
    public void AddBooking(Booking booking)
    {
        _context.Bookings.Add(booking);
    }

    public async Task<int> CountSimultaneousBookings(DateTime startTime, DateTime endTime)
    {
        return await _context.Bookings.Where(booking => booking.EndTime >= startTime && booking.StartTime < endTime).CountAsync();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}

