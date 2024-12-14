using System;
using BookingApplication.Core.Repositories;
using BookingApplication.Infrastructure.Services;
using BookingApplication.Infrastructure.Validators;
using BookingApplication.Infrastructure.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using BookingApplication.Test.API.Helpers;

namespace BookingApplication.Test.Infrastructure.Services;

public class BookingServiceTest
{
    private readonly Mock<ILogger<BookingService>> _mockLogger;
    private readonly Mock<IBookingRepository> _mockBookingRepository;
    private readonly Mock<IBookingValidator> _mockBookingValidator;
    private readonly BookingService _bookingServiceSut;
    public BookingServiceTest()
    {
        _mockLogger = new Mock<ILogger<BookingService>>();
        _mockBookingRepository = new Mock<IBookingRepository>();
        _mockBookingValidator = new Mock<IBookingValidator>();
        _bookingServiceSut = new BookingService(_mockLogger.Object, _mockBookingRepository.Object, _mockBookingValidator.Object);
    }

    [Fact]
    public async Task CreateBooking_OnSuccess_ShouldReturnBookingId()
    {
        var bookingRequest = RequestAndResponseBodyHelper.CreateBookingRequest();
        var bookingResponseDto = await _bookingServiceSut.CreateBooking(bookingRequest);
        Assert.IsType<Guid>(bookingResponseDto.BookingId);
    }

    [Fact]
    public async Task CreateBooking_OnError_ForOutOfHoursBooking_ShouldThrowOutOfBusinessHoursBookingException()
    {
        var bookingRequest = RequestAndResponseBodyHelper.CreateBookingRequest();
        _mockBookingValidator.Setup(validator => validator.ValidateBookingTime(It.IsAny<DateTime>())).Throws(new OutOfBusinessHoursBookingException());
        var outOfBusinessHoursBookingException = await Assert.ThrowsAsync<OutOfBusinessHoursBookingException>(async () => await _bookingServiceSut.CreateBooking(bookingRequest));
        Assert.NotNull(outOfBusinessHoursBookingException);
    }

    [Fact]
    public async Task CreateBooking_OnError_ForBookingTimeForPast_ShouldThrowBookingBeforeCurrentTimeException()
    {
        var bookingRequest = RequestAndResponseBodyHelper.CreateBookingRequest();
        _mockBookingValidator.Setup(validator => validator.ValidateBookingTime(It.IsAny<DateTime>())).Throws(new BookingBeforeCurrentTimeException());
        var bookingBeforeCurrentTimeException = await Assert.ThrowsAsync<BookingBeforeCurrentTimeException>(async () => await _bookingServiceSut.CreateBooking(bookingRequest));
        Assert.NotNull(bookingBeforeCurrentTimeException);
    }

    [Fact]
    public async Task CreateBooking_OnError_ForMoreThanMaximumSimultaneousBooking_ShouldThrowBookingCapacityExceededException()
    {
        var bookingRequest = RequestAndResponseBodyHelper.CreateBookingRequest();
        _mockBookingValidator.Setup(validator => validator.IsValidSimultaneousBookings(It.IsAny<DateTime>(), It.IsAny<IBookingRepository>())).ThrowsAsync(new BookingCapacityExceededException());
        var bookingCapacityExceededException = await Assert.ThrowsAsync<BookingCapacityExceededException>(async () => await _bookingServiceSut.CreateBooking(bookingRequest));
        Assert.NotNull(bookingCapacityExceededException);
    }

}
