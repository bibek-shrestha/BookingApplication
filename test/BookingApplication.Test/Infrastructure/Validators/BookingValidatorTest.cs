using System;
using BookingApplication.Core.Repositories;
using BookingApplication.Infrastructure.Exceptions;
using BookingApplication.Infrastructure.Validators;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using Moq;

namespace BookingApplication.Test.Infrastructure.Validators;

public class BookingValidatorTest
{
    private readonly Mock<ILogger<BookingValidator>> _mockLogger;
    private readonly TimeProvider _fakeTimeProvider;
    private readonly BookingValidator _bookingValidatorSut;
    public BookingValidatorTest()
    {
        _mockLogger = new Mock<ILogger<BookingValidator>>();
        _fakeTimeProvider = new FakeTimeProvider();
        _bookingValidatorSut = new BookingValidator(_mockLogger.Object, _fakeTimeProvider);
    }
    [Fact]
    public void ValidateBookingTime_OnSuccess_ShouldRunWithoutException()
    {
        _bookingValidatorSut.ValidateBookingTime(new DateTime(2024, 8, 17, 11, 0, 0));
    }

    [Fact]
    public void ValidateBookingTime_OnOutOfHourBookings_ShouldThrowOutOfBusinessHoursBookingException()
    {
        var outOfBusinessHoursBookingException = Assert.Throws<OutOfBusinessHoursBookingException>(() => _bookingValidatorSut.ValidateBookingTime(new DateTime(2024, 8, 17, 8, 0, 0)));
        Assert.NotNull(outOfBusinessHoursBookingException);
    }

    [Fact]
    public void ValidateBookingTime_OnOutOfHourBookings_ShouldThrowBookingBeforeCurrentTimeException()
    {
        var bookingBeforeCurrentTimeException = Assert.Throws<BookingBeforeCurrentTimeException>(() => _bookingValidatorSut.ValidateBookingTime(new DateTime(2024, 8, 17, 9, 0, 0)));
        Assert.NotNull(bookingBeforeCurrentTimeException);
    }

    [Fact]
    public async Task ValidateSimultaneousBookings_OnSuccess_ReturnTrue()
    {
        var bookingDate = new DateTime(2004, 8, 17, 11, 0, 0);
        var mockBookingRepository = new Mock<IBookingRepository>();
        mockBookingRepository.Setup(bookingRepository => bookingRepository.CountSimultaneousBookings(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(3);
        var isValid = await _bookingValidatorSut.IsValidSimultaneousBookings(bookingDate, mockBookingRepository.Object);
        Assert.True(isValid);
    }

    [Fact]
    public async Task ValidateSimultaneousBookings_OnError_ShouldThrowBookingCapacityExceededException()
    {
        var bookingDate = new DateTime(2004, 8, 17, 11, 0, 0);
        var mockBookingRepository = new Mock<IBookingRepository>();
        mockBookingRepository.Setup(bookingRepository => bookingRepository.CountSimultaneousBookings(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(4);
        var bookingCapacityExceededException = await Assert.ThrowsAsync<BookingCapacityExceededException>(async () => await _bookingValidatorSut.IsValidSimultaneousBookings(bookingDate, mockBookingRepository.Object));
        Assert.NotNull(bookingCapacityExceededException);
    }

}
