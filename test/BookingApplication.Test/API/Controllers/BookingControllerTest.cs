using System;
using BookingApplication.API.Controllers;
using BookingApplication.Infrastructure.Exceptions;
using BookingApplication.Infrastructure.Models;
using BookingApplication.Infrastructure.Services;
using BookingApplication.Test.API.Helpers;
using FluentValidation;
using FluentValidation.Results;
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
    private readonly Mock<IValidator<BookingCreationDto>> _mockValidator;

    public BookingControllerTest()
    {
        _mockLogger = new Mock<ILogger<BookingController>>();
        _mockBookingService = new Mock<IBookingService>();
        _mockValidator = new Mock<IValidator<BookingCreationDto>>();
        _bookingControllerSut = new BookingController(_mockLogger.Object, _mockBookingService.Object, _mockValidator.Object);
    }

    [Fact]
    public async Task Given_ValidBookingRequest_When_CreatingBooking_Then_ReturnsStatusCode200()
    {
        //Arrange
        var bookingRequest = RequestAndResponseBodyHelper.CreateBookingRequest();
        var expectedResponse = RequestAndResponseBodyHelper.GetBookingResponseDto();
        SetupValidator();
        _mockBookingService.Setup(service => service.CreateBooking(bookingRequest)).ReturnsAsync(expectedResponse);
        
        //Act
        var result = (OkObjectResult)await _bookingControllerSut.CreateBooking(bookingRequest);
        
        //Assert
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
    }

    [Fact]
    public async Task Given_ValidBookingRequest_When_CreatingBooking_Then_ReturnsBookingId()
    {
        //Arrange
        var bookingRequest = RequestAndResponseBodyHelper.CreateBookingRequest();
        var expectedReponse = RequestAndResponseBodyHelper.GetBookingResponseDto();
        SetupValidator();
        _mockBookingService.Setup(service => service.CreateBooking(bookingRequest)).ReturnsAsync(expectedReponse);

        //Act
        var result = (OkObjectResult)await _bookingControllerSut.CreateBooking(bookingRequest);

        //Assert
        var responseBody = (BookingResponseDto)Assert.IsType<BookingResponseDto>(result.Value);
        Assert.Equal(expectedReponse.BookingId, responseBody.BookingId);
    }

    [Fact]
    public async Task Given_OutOfBusinessHoursBookingRequest_When_CreatingBooking_Then_ReturnsStatusCode400()
    {
        //Arrange
        var bookingRequest = RequestAndResponseBodyHelper.CreateBookingRequest();
        SetupValidator();
        _mockBookingService.Setup(service => service.CreateBooking(bookingRequest)).Throws(new OutOfBusinessHoursBookingException());

        //Act
        var result = await _bookingControllerSut.CreateBooking(bookingRequest);
        
        //Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task Given_BookingRequestInPastTime_When_CreatingBooking_Then_ReturnsStatusCode400()
    {
        //Arrange
        var bookingRequest = RequestAndResponseBodyHelper.CreateBookingRequestForOutOfTimeBookingException();
        SetupValidator();
        _mockBookingService.Setup(service => service.CreateBooking(bookingRequest)).Throws(new BookingBeforeCurrentTimeException());
        
        //Act
        var result = await _bookingControllerSut.CreateBooking(bookingRequest);
        
        //Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task Given_BookingRequestWithNotEnoughBuffer_When_CreatingBooking_Then_ReturnsStatusCode400()
    {
        //Arrange
        var bookingRequest = RequestAndResponseBodyHelper.CreateBookingRequestForOutOfTimeBookingException();
        SetupValidator();
        _mockBookingService.Setup(service => service.CreateBooking(bookingRequest)).Throws(new BookingBufferTimeNotMetException());
        
        //Act
        var result = await _bookingControllerSut.CreateBooking(bookingRequest);
        
        //Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task Given_BookingRequestWithExceededCapacity_When_CreatingBooking_Then_ReturnsStatusCode409()
    {
        //Arrange
        var bookingRequest = RequestAndResponseBodyHelper.CreateBookingRequest();
        SetupValidator();
        _mockBookingService.Setup(service => service.CreateBooking(bookingRequest)).Throws(new BookingCapacityExceededException());
        
        //Act
        var result = await _bookingControllerSut.CreateBooking(bookingRequest);
        
        //Assert
        var conflictResult = Assert.IsType<ConflictObjectResult>(result);
        Assert.Equal(StatusCodes.Status409Conflict, conflictResult.StatusCode);
    }

    private void SetupValidator()
    {
        var validationResult = new ValidationResult();
        _mockValidator.Setup(validator => validator.ValidateAsync(It.IsAny<BookingCreationDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);
    }
}
