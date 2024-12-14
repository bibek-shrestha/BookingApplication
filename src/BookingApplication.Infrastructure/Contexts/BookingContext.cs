using System;
using BookingApplication.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingApplication.Infrastructure.Contexts;

public class BookingContext : DbContext
{
    public BookingContext(DbContextOptions<BookingContext> options) : base(options)
    { }

    public DbSet<Booking> Bookings { get; set; }

}
