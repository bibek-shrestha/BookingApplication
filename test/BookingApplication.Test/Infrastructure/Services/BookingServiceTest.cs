using System;
using BookingApplication.Core.Repositories;
using BookingApplication.Infrastructure.Services;
using BookingApplication.Infrastructure.Validators;
using BookingApplication.Infrastructure.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using BookingApplication.Test.API.Helpers;
using BookingApplication.Infrastructure.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using BookingApplication.Test.Infrastructure.Helpers;

namespace BookingApplication.Test.Infrastructure.Services;

public class BookingServiceTest
{
    private readonly Mock<ILogger<BookingService>> _mockLogger;
    private readonly Mock<IBookingRepository> _mockBookingRepository;
    private readonly Mock<IBookingValidator> _mockBookingValidator;
    private readonly BookingService _bookingServiceSut;
    public BookingServiceTest()
    {
        var options = BookingConfigOptionHelper.configureBookingOption();
        _mockLogger = new Mock<ILogger<BookingService>>();
        _mockBookingRepository = new Mock<IBookingRepository>();
        _mockBookingValidator = new Mock<IBookingValidator>();
        _bookingServiceSut = new BookingService(_mockLogger.Object, _mockBookingRepository.Object, _mockBookingValidator.Object, options);
    }

    [Fact]
    public async Task Given_ValidBookingRequest_When_CreateBookingIsCalled_Then_ReturnsBookingId()
    {
        //Arrange
        var bookingRequest = RequestAndResponseBodyHelper.CreateBookingRequest();

        //Act
        var bookingResponseDto = await _bookingServiceSut.CreateBooking(bookingRequest);

        //Assert
        Assert.IsType<Guid>(bookingResponseDto.BookingId);
    }

    [Fact]
    public async Task Given_BookingRequestWithOutOfBusinessHours_When_CreateBookingIsCalled_Then_ThrowsOutOfBusinessHoursBookingException()
    {
        //Arrange
        var bookingRequest = RequestAndResponseBodyHelper.CreateBookingRequest();
        _mockBookingValidator.Setup(validator => validator.ValidateBookingTime(It.IsAny<DateTime>())).Throws(new OutOfBusinessHoursBookingException());

        //Act and Assert
        var outOfBusinessHoursBookingException = await Assert.ThrowsAsync<OutOfBusinessHoursBookingException>(async () => await _bookingServiceSut.CreateBooking(bookingRequest));
        Assert.NotNull(outOfBusinessHoursBookingException);
    }

    [Fact]
    public async Task Given_BookingRequestWithPastTime_When_CreateBookingIsCalled_Then_ThrowsBookingBeforeCurrentTimeException()
    {
        //Arrange
        var bookingRequest = RequestAndResponseBodyHelper.CreateBookingRequest();
        _mockBookingValidator.Setup(validator => validator.ValidateBookingTime(It.IsAny<DateTime>())).Throws(new BookingBeforeCurrentTimeException());

        //Act and Assert
        var bookingBeforeCurrentTimeException = await Assert.ThrowsAsync<BookingBeforeCurrentTimeException>(async () => await _bookingServiceSut.CreateBooking(bookingRequest));
        Assert.NotNull(bookingBeforeCurrentTimeException);
    }

    [Fact]
    public async Task Given_BookingRequestWithNotEnoughBufferTime_When_CreateBookingIsCalled_Then_ThrowsBookingBufferTimeNotMetException()
    {
        //Arrange
        var bookingRequest = RequestAndResponseBodyHelper.CreateBookingRequest();
        _mockBookingValidator.Setup(validator => validator.ValidateBookingTime(It.IsAny<DateTime>())).Throws(new BookingBufferTimeNotMetException());

        //Act and Assert
        var bookingBufferTimeNotMetException = await Assert.ThrowsAsync<BookingBufferTimeNotMetException>(async () => await _bookingServiceSut.CreateBooking(bookingRequest));
        Assert.NotNull(bookingBufferTimeNotMetException);
    }

    [Fact]
    public async Task Given_BookingRequestExceedingMaxSimultaneousBookings_When_CreateBookingIsCalled_Then_ThrowsBookingCapacityExceededException()
    {
        //Arrange
        var bookingRequest = RequestAndResponseBodyHelper.CreateBookingRequest();
        _mockBookingValidator.Setup(validator => validator.IsValidSimultaneousBookings(It.IsAny<DateTime>(), It.IsAny<IBookingRepository>())).ThrowsAsync(new BookingCapacityExceededException());

        //Act and Assert
        var bookingCapacityExceededException = await Assert.ThrowsAsync<BookingCapacityExceededException>(async () => await _bookingServiceSut.CreateBooking(bookingRequest));
        Assert.NotNull(bookingCapacityExceededException);
    }
}
