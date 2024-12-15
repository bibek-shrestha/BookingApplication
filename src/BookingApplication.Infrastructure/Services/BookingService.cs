using System;
using System.Globalization;
using BookingApplication.Core.Entities;
using BookingApplication.Core.Repositories;
using BookingApplication.Infrastructure.Configs;
using BookingApplication.Infrastructure.Exceptions;
using BookingApplication.Infrastructure.Models;
using BookingApplication.Infrastructure.Validators;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BookingApplication.Infrastructure.Services;

public class BookingService : IBookingService
{
    private readonly ILogger<BookingService> _logger;
    private readonly IBookingRepository _bookingRepository;
    private readonly IBookingValidator _bookingValidator;
    private readonly BookingConfigOption _bookingConfig;

    public BookingService(
        ILogger<BookingService> logger,
        IBookingRepository bookingRepository,
        IBookingValidator bookingValidator,
        IOptions<BookingConfigOption> bookingConfig)
    {
        _logger = logger;
        _bookingRepository = bookingRepository;
        _bookingValidator = bookingValidator;
        _bookingConfig = bookingConfig.Value;
    }
    public async Task<BookingResponseDto> CreateBooking(BookingCreationDto bookingRequest)
    {
        var bookingTime = ParseBookingTime(bookingRequest.BookingTime);
        IEnumerable<Convener> availableConveners;
        try
        {
            _logger.LogDebug("Validating booking for {Name} at {BookingTime}.", bookingRequest.Name, bookingTime);
            _bookingValidator.ValidateBookingTime(bookingTime);
            availableConveners = await _bookingValidator.ValidateSimultaneousBookingAndGetAvailableConvenors(bookingTime, _bookingRepository);
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
        Convener selectedConvener = SelectDefaultOrFirstConvenor(availableConveners);
        var newBooking = CreateAndSaveBooking(bookingTime, bookingRequest.Name, selectedConvener);
        return new BookingResponseDto(newBooking.Id);
    }

    private Booking CreateAndSaveBooking(DateTime bookingTime, string name, Convener convener)
    {
        _logger.LogInformation("Creating a booking for '{Name}' at {BookingTime}.", name, bookingTime);
        var endTime = bookingTime.AddMinutes(_bookingConfig.BookingDuration);
        var newBooking = new Booking(bookingTime, endTime, name, convener);
        _bookingRepository.AddBooking(newBooking);
        _bookingRepository.SaveChangesAsync().Wait();
        _logger.LogInformation("Successfully created a booking for '{Name}' at {BookingTime} with ID {BookingId}.", name, bookingTime, newBooking.Id);
        return newBooking;

    }

    private DateTime ParseBookingTime(string bookingTimeString)
    {
        return DateTime.ParseExact(bookingTimeString, "HH:mm", CultureInfo.InvariantCulture);
    }

    private Convener SelectDefaultOrFirstConvenor(IEnumerable<Convener> conveners)
    {
        return conveners.FirstOrDefault(Convener.FIRST_CONVENER);
    }
}
