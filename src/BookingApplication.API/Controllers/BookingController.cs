using BookingApplication.Infrastructure.Exceptions;
using BookingApplication.Infrastructure.Models;
using BookingApplication.Infrastructure.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace BookingApplication.API.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingController : ControllerBase
{
    private readonly ILogger<BookingController> _logger;

    private readonly IBookingService _bookingService;

    private readonly IValidator<BookingCreationDto> _validator;

    public BookingController(
        ILogger<BookingController> logger,
        IBookingService bookingService,
        IValidator<BookingCreationDto> validator)
    {
        _logger = logger;
        _bookingService = bookingService;
        _validator = validator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] BookingCreationDto bookingRequest)
    {
        _logger.LogInformation("Request to create booking received with data: {@BookingRequest}", bookingRequest);
        var validationResult = await _validator.ValidateAsync(bookingRequest);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }
        try
        {
            var bookingResponseDto = await _bookingService.CreateBooking(bookingRequest);
            return Ok(bookingResponseDto);
        }
        catch (BookingException ex)
            when (
                ex is OutOfBusinessHoursBookingException ||
                ex is BookingBeforeCurrentTimeException ||
                ex is BookingBufferTimeNotMetException)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (BookingCapacityExceededException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }
}
