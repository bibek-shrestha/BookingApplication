using System;
using System.Globalization;
using BookingApplication.Core.Entities;
using BookingApplication.Core.Repositories;
using BookingApplication.Infrastructure.Exceptions;
using BookingApplication.Infrastructure.Models;
using BookingApplication.Infrastructure.Validators;
using Microsoft.Extensions.Logging;

namespace BookingApplication.Infrastructure.Services;

public class BookingService : IBookingService
{
    private readonly ILogger<BookingService> _logger;
    private readonly IBookingRepository _bookingRepository;
    private readonly IBookingValidator _bookingValidator;

    public BookingService(
        ILogger<BookingService> logger,
        IBookingRepository bookingRepository,
        IBookingValidator bookingValidator)
    {
        _logger = logger;
        _bookingRepository = bookingRepository;
        _bookingValidator = bookingValidator;
    }
    public async Task<BookingResponseDto> CreateBooking(BookingCreationDto bookingRequest)
    {
        var bookingTime = DateTime.ParseExact(bookingRequest.BookingTime, "HH:mm", CultureInfo.InvariantCulture);
        try
        {
            _logger.LogDebug("Validating booking for {Name} at {BookingTime}.", bookingRequest.Name, bookingTime);
            _bookingValidator.ValidateBookingTime(bookingTime);
            await _bookingValidator.IsValidSimultaneousBookings(bookingTime, _bookingRepository);
        }
        catch (OutOfBusinessHoursBookingException)
        {
            throw;
        }
        catch (BookingBeforeCurrentTimeException)
        {
            throw;
        }
        catch (BookingCapacityExceededException)
        {
            throw;
        }
        _logger.LogInformation("Creating a booking for '{Name}' at {BookingTime}.", bookingRequest.Name, bookingTime);
        var newBooking = new Booking(bookingTime, bookingRequest.Name);
        _bookingRepository.AddBooking(newBooking);
        await _bookingRepository.SaveChangesAsync();
        _logger.LogInformation("Successfully created a booking for '{Name}' at {BookingTime} with ID {BookingId}.", bookingRequest.Name, bookingTime, newBooking.Id);
        return new BookingResponseDto(newBooking.Id);
    }
}