using System;
using BookingApplication.Core.Entities;

namespace BookingApplication.Test.Infrastructure.Helpers;

public class BookingHelper
{
    public static Booking CreateNewBooking()
    {
        return new Booking(new DateTime(2004, 8, 17, 10, 0, 0), "John Smith");
    }

    public static DateTime GetNewBookingTime()
    {
        return new DateTime(2004, 8, 17, 10, 0, 0);
    }

}