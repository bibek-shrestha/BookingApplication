using BookingApplication.Infrastructure.Exceptions;
using BookingApplication.Infrastructure.Models;
using BookingApplication.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingApplication.API.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingController : ControllerBase
{
    private readonly ILogger<BookingController> _logger;

    private readonly IBookingService _bookingService;

    public BookingController(ILogger<BookingController> logger, IBookingService bookingService)
    {
        _logger = logger;
        _bookingService = bookingService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] BookingCreationDto bookingRequest)
    {
        _logger.LogInformation("Request to create booking received with data: {@BookingRequest}", bookingRequest);
        try
        {
            var bookingResponseDto = await _bookingService.CreateBooking(bookingRequest);
            return Ok(bookingResponseDto);
        }
        catch (OutOfBusinessHoursBookingException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (BookingBeforeCurrentTimeException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (BookingCapacityExceededException ex)
        {
            return Conflict(ex.Message);
        }
    }
}
