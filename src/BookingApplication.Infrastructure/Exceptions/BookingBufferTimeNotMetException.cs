using System;

namespace BookingApplication.Infrastructure.Exceptions;

public class BookingBufferTimeNotMetException : BookingException
{
    public BookingBufferTimeNotMetException()
    { }

    public BookingBufferTimeNotMetException(string message) : base(message)
    { }

    public BookingBufferTimeNotMetException(string message, Exception innerException) : base(message, innerException)
    { }
}
