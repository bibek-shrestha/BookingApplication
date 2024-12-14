using System;

namespace BookingApplication.Infrastructure.Exceptions;

public class OutOfBusinessHoursBookingException : BookingException
{
    public OutOfBusinessHoursBookingException()
    { }

    public OutOfBusinessHoursBookingException(string message) : base(message)
    { }

    public OutOfBusinessHoursBookingException(string message, Exception innerException) : base(message, innerException)
    { }
}
