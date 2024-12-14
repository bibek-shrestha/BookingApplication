using System;
using BookingApplication.API.Controllers;
using BookingApplication.Infrastructure.Exceptions;
using BookingApplication.Infrastructure.Models;
using BookingApplication.Infrastructure.Services;
using BookingApplication.Test.API.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookingApplication.Test.API.Controllers;

public class BookingControllerTest
{
    private readonly Mock<ILogger<BookingController>> _mockLogger;
    private readonly Mock<IBookingService> _mockBookingService;
    private readonly BookingController _bookingControllerSut;

    public BookingControllerTest()
    {
        _mockLogger = new Mock<ILogger<BookingController>>();
        _mockBookingService = new Mock<IBookingService>();
        _bookingControllerSut = new BookingController(_mockLogger.Object, _mockBookingService.Object);
    }

    [Fact]
    public async Task Should_Return_StatusCode200_OnSuccess()
    {
        var bookingRequest = RequestAndResponseBodyHelper.CreateBookingRequest();
        var expectedResponse = RequestAndResponseBodyHelper.GetBookingResponseDto();
        _mockBookingService.Setup(service => service.CreateBooking(bookingRequest)).ReturnsAsync(expectedResponse);
        var result = (OkObjectResult)await _bookingControllerSut.CreateBooking(bookingRequest);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
    }

    [Fact]
    public async Task PostBooking_ShouldReturn_BookingId_OnSuccess()
    {
        var bookingRequest = RequestAndResponseBodyHelper.CreateBookingRequest();
        var expectedReponse = RequestAndResponseBodyHelper.GetBookingResponseDto();
        _mockBookingService.Setup(service => service.CreateBooking(bookingRequest)).ReturnsAsync(expectedReponse);
        var result = (OkObjectResult)await _bookingControllerSut.CreateBooking(bookingRequest);
        var responseBody = (BookingResponseDto)Assert.IsType<BookingResponseDto>(result.Value);
        Assert.Equal(expectedReponse.BookingId, responseBody.BookingId);
    }

    [Fact]
    public async Task PostBooking_ShouldReturn_Status400_OnError_ForOutOfHours()
    {
        var bookingRequest = RequestAndResponseBodyHelper.CreateBookingRequest();
        _mockBookingService.Setup(service => service.CreateBooking(bookingRequest)).Throws(new OutOfBusinessHoursBookingException());
        var result = await _bookingControllerSut.CreateBooking(bookingRequest);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task PostBooking_ShouldReturn_Status400_OnError_ForPastTimeBooking()
    {
        var bookingRequest = RequestAndResponseBodyHelper.CreateBookingRequestForOutOfTimeBookingException();
        _mockBookingService.Setup(service => service.CreateBooking(bookingRequest)).Throws(new BookingBeforeCurrentTimeException());
        var result = await _bookingControllerSut.CreateBooking(bookingRequest);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task PostBooking_ShouldReturn_Status400_OnError_ForBookingWithNotEnoughBuffer()
    {
        var bookingRequest = RequestAndResponseBodyHelper.CreateBookingRequestForOutOfTimeBookingException();
        _mockBookingService.Setup(service => service.CreateBooking(bookingRequest)).Throws(new BookingBufferTimeNotMetException());
        var result = await _bookingControllerSut.CreateBooking(bookingRequest);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task PostBooking_ShouldReturn_Status409_OnError_ForMoreThanFourBookings()
    {
        var bookingRequest = RequestAndResponseBodyHelper.CreateBookingRequest();
        _mockBookingService.Setup(service => service.CreateBooking(bookingRequest)).Throws(new BookingCapacityExceededException());
        var result = await _bookingControllerSut.CreateBooking(bookingRequest);
        var conflictResult = Assert.IsType<ConflictObjectResult>(result);
        Assert.Equal(StatusCodes.Status409Conflict, conflictResult.StatusCode);
    }

}
