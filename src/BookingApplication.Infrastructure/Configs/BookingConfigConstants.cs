using System;

namespace BookingApplication.Infrastructure.Configs;

public class BookingConfigConstants
{
    public const string BUSINESS_HOURS_START_TIME = "09:00";
    public const string BUSINESS_HOURS_END_TIME = "16:00";
    public const int BOOKING_DURATION_IN_MINUTES = 59;
    public const int BUFFER_FOR_BOOKING = 0;
    public const int TIME_BEFORE_BUSINESS_HOURS_BEFORE_LAST_BOOKING = 60;
    public const int MAXIMUM_SIMULTANEOUS_BOOKINGS = 4;

}
