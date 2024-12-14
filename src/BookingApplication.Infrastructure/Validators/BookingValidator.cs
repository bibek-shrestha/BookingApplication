using System;
using BookingApplication.Core.Repositories;
using BookingApplication.Infrastructure.Exceptions;
using Microsoft.Extensions.Logging;

namespace BookingApplication.Infrastructure.Validators;

public class BookingValidator : IBookingValidator
{
    private const string BUSINESS_HOURS_START_TIME = "09:00";
    private const string BUSINESS_HOURS_END_TIME = "16:00";
    private const int BOOKING_DURATION_IN_MINUTES = 59;
    private readonly ILogger<BookingValidator> _logger;
    private readonly TimeProvider _timeProvider;
    public BookingValidator(ILogger<BookingValidator> logger, TimeProvider timeProvider)
    {
        _logger = logger;
        _timeProvider = timeProvider;
    }
    public void ValidateBookingTime(DateTime requestedBookingDateTime)
    {
        var bookingTime = TimeOnly.FromDateTime(requestedBookingDateTime);
        if (!IsBookingWithInBusinessHours(bookingTime))
        {
            _logger.LogWarning("Booking time ({BookingTime}) is outside business hours.", bookingTime);
            throw new OutOfBusinessHoursBookingException("Booking cannot be created outside business hours.");
        }
        if (!IsBookingAfterCurrentTime(bookingTime))
        {
            _logger.LogWarning("Booking time ({BookingTime}) is for a past time.", bookingTime);
            throw new BookingBeforeCurrentTimeException("Booking for times before current time is not allowed.");
        }
    }

    public async Task<bool> IsValidSimultaneousBookings(DateTime startTime, IBookingRepository bookingRepository)
    {
        var endTime = startTime.AddMinutes(BOOKING_DURATION_IN_MINUTES);
        var numberOfSimultaneousBookings = await bookingRepository.CountSimultaneousBookings(startTime, endTime);
        if (numberOfSimultaneousBookings > 3)
        {
            _logger.LogWarning("Booking conflict detected for booking at {StartTime} to {EndTime}. Number of simultaneous bookings: {ConflictCount}"
               , startTime, endTime, numberOfSimultaneousBookings);
            throw new BookingCapacityExceededException("Booking exceeds total number of simultaneous bookings.");
        }
        return true;
    }

    private bool IsBookingWithInBusinessHours(TimeOnly bookingTime)
    {
        return TimeOnly.Parse(BUSINESS_HOURS_END_TIME) >= bookingTime
        && TimeOnly.Parse(BUSINESS_HOURS_START_TIME) <= bookingTime;
    }

    private bool IsBookingAfterCurrentTime(TimeOnly bookingTime)
    {
        var currentDateTime = _timeProvider.GetLocalNow().LocalDateTime;
        var currentTimeOnly = TimeOnly.FromDateTime(currentDateTime);
        return currentTimeOnly <= bookingTime;
    }
}
