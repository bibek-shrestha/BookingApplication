using System;

namespace BookingApplication.Infrastructure.Exceptions;

public class BookingCapacityExceededException : BookingException
{
    public BookingCapacityExceededException()
    { }

    public BookingCapacityExceededException(string message) : base(message)
    { }

    public BookingCapacityExceededException(string message, Exception innerException) : base(message, innerException)
    { }
}
