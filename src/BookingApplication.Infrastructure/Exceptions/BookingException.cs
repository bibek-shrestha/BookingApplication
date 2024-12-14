using System;

namespace BookingApplication.Infrastructure.Exceptions;

public class BookingException : Exception
{

    public BookingException()
    { }

    public BookingException(string message) : base(message)
    { }

    public BookingException(string message, Exception innerException) : base(message, innerException)
    { }

}
