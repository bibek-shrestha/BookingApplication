using System;
using BookingApplication.Core.Entities;
using BookingApplication.Core.Repositories;

namespace BookingApplication.Infrastructure.Validators;

public interface IBookingValidator
{
    void ValidateBookingTime(DateTime bookingTime);

    Task<IEnumerable<Convener>> ValidateSimultaneousBookingAndGetAvailableConvenors(DateTime bookingTime, IBookingRepository bookingRepository);
}
