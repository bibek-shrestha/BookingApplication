using System;
using BookingApplication.Infrastructure.Models;

namespace BookingApplication.Infrastructure.Services;

public interface IBookingService
{
    Task<BookingResponseDto> CreateBooking(BookingCreationDto bookingRequest);
}
