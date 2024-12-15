using System;
using BookingApplication.Core.Entities;
using BookingApplication.Core.Repositories;
using BookingApplication.Infrastructure.Configs;
using BookingApplication.Infrastructure.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BookingApplication.Infrastructure.Validators;

public class BookingValidator : IBookingValidator
{
    private readonly ILogger<BookingValidator> _logger;
    private readonly TimeProvider _timeProvider;
    private readonly BookingConfigOption _bookingConfig;
    public BookingValidator(
        ILogger<BookingValidator> logger,
        TimeProvider timeProvider,
        IOptions<BookingConfigOption> bookingConfig)
    {
        _logger = logger;
        _timeProvider = timeProvider;
        _bookingConfig = bookingConfig.Value;
    }
    public void ValidateBookingTime(DateTime requestedBookingDateTime)
    {
        var bookingTime = TimeOnly.FromDateTime(requestedBookingDateTime);
        if (!IsBookingWithInBusinessHours(bookingTime))
        {
            _logger.LogWarning("Booking time ({BookingTime}) is outside business hours.", bookingTime);
            throw new OutOfBusinessHoursBookingException($"Bookings can only be made during business hours. Please choose a time within the designated hours of operation ({_bookingConfig.BusinessHoursStartTime} - {_bookingConfig.BusinessHoursEndTime}).");
        }
        if (!IsBookingAfterCurrentTime(bookingTime))
        {
            _logger.LogWarning("Booking time ({BookingTime}) is for a past time.", bookingTime);
            throw new BookingBeforeCurrentTimeException("Booking cannot be made for a time in the past. Please select a future time for your booking.");
        }
        if (!IsWithinBufferTime(bookingTime))
        {
            _logger.LogWarning("Booking time ({BookingTime}) is for a past time.", bookingTime);
            throw new BookingBufferTimeNotMetException($"Bookings must be made at least {_bookingConfig.BufferForBooking} minutes in advance of the requested time.");
        }
    }

    public async Task<IEnumerable<Convener>> ValidateSimultaneousBookingAndGetAvailableConvenors(DateTime startTime, IBookingRepository bookingRepository)
    {
        var endTime = startTime.AddMinutes(_bookingConfig.BookingDuration);
        var numberOfSimultaneousBookings = await bookingRepository.GetBookingsForTimeRangeAsync(startTime, endTime);
        var unavailableConveners = numberOfSimultaneousBookings.Select(booking => booking.Convener).Distinct();
        if (unavailableConveners.Count() == _bookingConfig.MaximumSimultaneousBookings)
        {
            _logger.LogWarning("Booking conflict detected for booking at {StartTime} to {EndTime}. Number of simultaneous bookings: {ConflictCount}"
               , startTime, endTime, numberOfSimultaneousBookings);
            throw new BookingCapacityExceededException("The booking exceeds the available capacity for this time. Please select a different time.");
        }
        return Enum.GetValues(typeof(Convener)).Cast<Convener>()
            .Where(c => !unavailableConveners.Contains(c)).ToList();
    }

    private bool IsBookingWithInBusinessHours(TimeOnly bookingTime)
    {
        return TimeOnly.Parse(_bookingConfig.BusinessHoursEndTime).AddMinutes(-_bookingConfig.TimeBeforeBusinessHoursForLastBooking) >= bookingTime
        && TimeOnly.Parse(_bookingConfig.BusinessHoursStartTime) <= bookingTime;
    }

    private bool IsBookingAfterCurrentTime(TimeOnly bookingTime)
    {
        var currentTime = GetCurrentTime();
        return currentTime <= bookingTime;
    }

    private bool IsWithinBufferTime(TimeOnly bookingTime)
    {
        var currentTime = GetCurrentTime();
        return currentTime.AddMinutes(_bookingConfig.BufferForBooking) <= bookingTime;
    }

    private TimeOnly GetCurrentTime()
    {
        var currentDateTime = _timeProvider.GetLocalNow().LocalDateTime;
        return TimeOnly.FromDateTime(currentDateTime);
    }
}
