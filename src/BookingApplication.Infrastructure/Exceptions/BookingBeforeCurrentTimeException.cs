using System;

namespace BookingApplication.Infrastructure.Exceptions;

public class BookingBeforeCurrentTimeException : BookingException
{
    public BookingBeforeCurrentTimeException()
    { }

    public BookingBeforeCurrentTimeException(string message) : base(message)
    { }

    public BookingBeforeCurrentTimeException(string message, Exception innerException) : base(message, innerException)
    { }
}
