using System;
using BookingApplication.Infrastructure.Models;

namespace BookingApplication.Test.API.Helpers;

public static class RequestAndResponseBodyHelper
{
    public static BookingCreationDto CreateBookingRequest()
    {
        return new BookingCreationDto("11:30", "John Smith");
    }

    public static BookingCreationDto CreateBookingRequestForOutOfTimeBookingException()
    {
        return new BookingCreationDto("16:30", "John Smith");
    }

    public static BookingCreationDto CreateBookingRequestForHoursBeforeCurrentTime()
    {
        return new BookingCreationDto("14:00", "John Smith");
    }

    public static BookingResponseDto GetBookingResponseDto()
    {
        return new BookingResponseDto(Guid.NewGuid());
    }
}