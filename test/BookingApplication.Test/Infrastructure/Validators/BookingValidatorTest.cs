using System;
using BookingApplication.Core.Repositories;
using BookingApplication.Infrastructure.Exceptions;
using BookingApplication.Infrastructure.Validators;
using BookingApplication.Test.Infrastructure.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using Moq;

namespace BookingApplication.Test.Infrastructure.Validators;

public class BookingValidatorTest
{
    private readonly Mock<ILogger<BookingValidator>> _mockLogger;
    private readonly FakeTimeProvider _fakeTimeProvider;
    private readonly BookingValidator _bookingValidatorSut;
    public BookingValidatorTest()
    {
        //Arrange
        var options = BookingConfigOptionHelper.configureBookingOption();
        _mockLogger = new Mock<ILogger<BookingValidator>>();
        _fakeTimeProvider = new FakeTimeProvider();
        _fakeTimeProvider.SetUtcNow(new DateTimeOffset(new DateTime(2004, 8, 17, 10, 0, 0)));
        _bookingValidatorSut = new BookingValidator(_mockLogger.Object, _fakeTimeProvider, options);
    }
    [Fact]
    public void Given_ValidBookingTime_When_ValidateBookingTimeIsCalled_Then_RunsWithoutException()
    {
        //Act
        _bookingValidatorSut.ValidateBookingTime(new DateTime(2004, 8, 17, 11, 0, 0));
    }

    [Fact]
    public void Given_BookingTimeOutsideBusinessHours_When_ValidateBookingTimeIsCalled_Then_ThrowsOutOfBusinessHoursBookingException()
    {
        //Act and Assert
        var outOfBusinessHoursBookingException = Assert.Throws<OutOfBusinessHoursBookingException>(() => _bookingValidatorSut.ValidateBookingTime(new DateTime(2004, 8, 17, 8, 0, 0)));
        Assert.NotNull(outOfBusinessHoursBookingException);
    }

    [Fact]
    public void Given_BookingTimeBeforeCurrentTime_When_ValidateBookingTimeIsCalled_Then_ThrowsBookingBeforeCurrentTimeException()
    {
        //Act and Assert
        var bookingBeforeCurrentTimeException = Assert.Throws<BookingBeforeCurrentTimeException>(() => _bookingValidatorSut.ValidateBookingTime(new DateTime(2004, 8, 17, 9, 0, 0)));
        Assert.NotNull(bookingBeforeCurrentTimeException);
    }

    [Fact]
    public void Given_BookingTimeWithNotEnoughBuffer_When_ValidateBookingTimeIsCalled_Then_ThrowsBookingBufferTimeNotMetException()
    {
        //Act and Assert
        var bookingBufferTimeNotMetException = Assert.Throws<BookingBufferTimeNotMetException>(() => _bookingValidatorSut.ValidateBookingTime(new DateTime(2004, 8, 17, 10, 12, 0)));
        Assert.NotNull(bookingBufferTimeNotMetException);
    }

    [Fact]
    public async Task Given_ValidBookingDateAndSimultaneousBookings_When_IsValidSimultaneousBookingsIsCalled_Then_ReturnsTrue()
    {
        //Arrange
        var bookingDate = new DateTime(2004, 8, 17, 11, 0, 0);
        var mockBookingRepository = new Mock<IBookingRepository>();
        mockBookingRepository.Setup(bookingRepository => bookingRepository.CountSimultaneousBookings(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(3);

        //Act
        var isValid = await _bookingValidatorSut.IsValidSimultaneousBookings(bookingDate, mockBookingRepository.Object);

        //Assert
        Assert.True(isValid);
    }

    [Fact]
    public async Task Given_BookingDateWithExcessSimultaneousBookings_When_IsValidSimultaneousBookingsIsCalled_Then_ThrowsBookingCapacityExceededException()
    {
        //Arrange
        var bookingDate = new DateTime(2004, 8, 17, 11, 0, 0);
        var mockBookingRepository = new Mock<IBookingRepository>();
        mockBookingRepository.Setup(bookingRepository => bookingRepository.CountSimultaneousBookings(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(4);

        //Act and Assert
        var bookingCapacityExceededException = await Assert.ThrowsAsync<BookingCapacityExceededException>(async () => await _bookingValidatorSut.IsValidSimultaneousBookings(bookingDate, mockBookingRepository.Object));
        Assert.NotNull(bookingCapacityExceededException);
    }

}
