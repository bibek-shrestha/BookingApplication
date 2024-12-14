using System;

namespace BookingApplication.Infrastructure.Configs;

public class BookingConfigOption
{
    public const string BookingConfig = "Booking";
    public string BusinessHoursStartTime { get; set; } = BookingConfigConstants.BUSINESS_HOURS_START_TIME;
    public string BusinessHoursEndTime { get; set; } = BookingConfigConstants.BUSINESS_HOURS_END_TIME;
    public int BookingDuration { get; set; } = BookingConfigConstants.BOOKING_DURATION_IN_MINUTES;
    public int BufferForBooking { get; set; } = BookingConfigConstants.BUFFER_FOR_BOOKING;
}
