using System;

namespace BookingApplication.Infrastructure.Configs;

public class BookingConfigOption
{
    public const string BookingConfig = "Booking";
    public string BusinessHoursStartTime { get; set; } = BookingConfigConstants.BUSINESS_HOURS_START_TIME;
    public string BusinessHoursEndTime { get; set; } = BookingConfigConstants.BUSINESS_HOURS_END_TIME;
    public int BookingDuration { get; set; } = BookingConfigConstants.BOOKING_DURATION_IN_MINUTES;
    public int BufferForBooking { get; set; } = BookingConfigConstants.BUFFER_FOR_BOOKING;
    public int TimeBeforeBusinessHoursForLastBooking { get; set;} = BookingConfigConstants.TIME_BEFORE_BUSINESS_HOURS_BEFORE_LAST_BOOKING;
    public int MaximumSimultaneousBookings { get; set; } = BookingConfigConstants.MAXIMUM_SIMULTANEOUS_BOOKINGS;
}
