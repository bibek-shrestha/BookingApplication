using System;
using BookingApplication.Core.Repositories;

namespace BookingApplication.Infrastructure.Validators;

public interface IBookingValidator
{
    void ValidateBookingTime(DateTime bookingTime);

    Task<bool> IsValidSimultaneousBookings(DateTime bookingTime, IBookingRepository bookingRepository);
}
