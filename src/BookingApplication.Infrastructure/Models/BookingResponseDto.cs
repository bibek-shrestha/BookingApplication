using System;

namespace BookingApplication.Infrastructure.Models;

public class BookingResponseDto
{
    public Guid BookingId { get; set; }

    public BookingResponseDto(Guid bookingId)
    {
        BookingId = bookingId;
    }
}