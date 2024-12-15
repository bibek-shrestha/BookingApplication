using System;
using BookingApplication.Core.Entities;
using BookingApplication.Infrastructure.Contexts;
using BookingApplication.Infrastructure.Repositories;
using BookingApplication.Test.Infrastructure.Helpers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BookingApplication.Test.Infrastructure.Repositories;

public class BookingRepositoryTest : IDisposable
{
    private readonly BookingContext _context;
    private readonly BookingRepository _bookingRepository;
    private const string IN_MEMORY_SQLITE_DATASOURCE = "DataSource=:memory:";
    public BookingRepositoryTest()
    {
        var sqliteConnection = new SqliteConnection(IN_MEMORY_SQLITE_DATASOURCE);
        sqliteConnection.Open();
        var options = new DbContextOptionsBuilder<BookingContext>()
            .UseSqlite(sqliteConnection)
            .Options;
        _context = new BookingContext(options);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
        _bookingRepository = new BookingRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public void Given_NewBooking_When_AddedToRepository_Then_TrackingStateIsAdded()
    {
        //Arrange
        var booking = BookingHelper.CreateNewBooking();

        //Act
        _bookingRepository.AddBooking(booking);

        //Assert
        var trackedBooking = _context.ChangeTracker.Entries<Booking>().FirstOrDefault();
        Assert.NotNull(trackedBooking);
        Assert.Equal(EntityState.Added, trackedBooking.State);
    }

    [Fact]
    public async void Given_NewBooking_When_SaveChangesAsyncIsCalled_Then_OneRecordSavedAndTrackingStateIsUnchanged()
    {
        //Arrange
        var booking = BookingHelper.CreateNewBooking();
        _bookingRepository.AddBooking(booking);

        //Act
        var numberOfSavedRecords = await _bookingRepository.SaveChangesAsync();

        //Assert
        var trackedBooking = _context.ChangeTracker.Entries<Booking>().FirstOrDefault();
        Assert.Equal(1, numberOfSavedRecords);
        Assert.NotNull(trackedBooking);
        Assert.Equal(EntityState.Unchanged, trackedBooking.State);
    }

    [Fact]
    public async void Given_ValidStartAndEndTime_When_GetBookingsForTimeRangeAsyncIsCalled_Then_LessThan4IsReturned()
    {
        var firstBooking = new Booking(new DateTime(2004, 8, 17, 10, 0, 0), new DateTime(2004, 8, 17, 10, 59, 0), "John Smith", Convener.FIRST_CONVENER);
        var secondBooking = new Booking(new DateTime(2004, 8, 17, 10, 0, 0), new DateTime(2004, 8, 17, 10, 59, 0), "John Smith", Convener.SECOND_CONVENER);
        var thirdBooking = new Booking(new DateTime(2004, 8, 17, 10, 0, 0), new DateTime(2004, 8, 17, 10, 59, 0), "John Smith", Convener.THIRD_CONVENER);
        var fourthBooking = new Booking(new DateTime(2004, 8, 17, 12, 0, 0), new DateTime(2004, 8, 17, 12, 59, 0), "John Smith", Convener.FIRST_CONVENER);
        _bookingRepository.AddBooking(firstBooking);
        _bookingRepository.AddBooking(secondBooking);
        _bookingRepository.AddBooking(thirdBooking);
        _bookingRepository.AddBooking(fourthBooking);

        await _bookingRepository.SaveChangesAsync();

        var startTime = new DateTime(2004, 8, 17, 10, 0, 0);
        var endTime = new DateTime(2004, 8, 17, 10, 59, 0);

        var availableConveners = await _bookingRepository.GetBookingsForTimeRangeAsync(startTime, endTime);
        Assert.Equal(3, availableConveners.Count());
    }

    [Fact]
    public async void Given_ValidStartAndEndTime_ForEdgeCase_When_GetBookingsForTimeRangeAsyncIsCalled_Then_LessThan4IsReturned()
    {
        var firstBooking = new Booking(new DateTime(2004, 8, 17, 14, 0, 0), new DateTime(2004, 8, 17, 14, 59, 0), "John Smith", Convener.FIRST_CONVENER);
        var secondBooking = new Booking(new DateTime(2004, 8, 17, 14, 10, 0), new DateTime(2004, 8, 17, 15, 09, 0), "John Smith", Convener.SECOND_CONVENER);
        var fourthBooking = new Booking(new DateTime(2004, 8, 17, 15, 0, 0), new DateTime(2004, 8, 17, 15, 59, 0), "John Smith", Convener.FIRST_CONVENER);
        _bookingRepository.AddBooking(firstBooking);
        _bookingRepository.AddBooking(secondBooking);
        _bookingRepository.AddBooking(fourthBooking);

        await _bookingRepository.SaveChangesAsync();

        var startTime = new DateTime(2004, 8, 17, 14, 30, 0);
        var endTime = new DateTime(2004, 8, 17, 15, 29, 0);

        var availableConveners = await _bookingRepository.GetBookingsForTimeRangeAsync(startTime, endTime);
        Assert.Equal(3, availableConveners.Count());
    }

    [Fact]
    public async void Given_OverlappingStartAndEndTime_When_GetBookingsForTimeRangeAsyncIsCalled_Then_GreaterThan4IsReturned()
    {
        var firstBooking = new Booking(new DateTime(2004, 8, 17, 10, 0, 0), new DateTime(2004, 8, 17, 10, 59, 0), "John Smith", Convener.FIRST_CONVENER);
        var secondBooking = new Booking(new DateTime(2004, 8, 17, 10, 0, 0), new DateTime(2004, 8, 17, 10, 59, 0), "John Smith", Convener.SECOND_CONVENER);
        var thirdBooking = new Booking(new DateTime(2004, 8, 17, 10, 0, 0), new DateTime(2004, 8, 17, 10, 59, 0), "John Smith", Convener.THIRD_CONVENER);
        var fourthBooking = new Booking(new DateTime(2004, 8, 17, 10, 0, 0), new DateTime(2004, 8, 17, 10, 59, 0), "John Smith", Convener.FOURTH_CONVENER);
        _bookingRepository.AddBooking(firstBooking);
        _bookingRepository.AddBooking(secondBooking);
        _bookingRepository.AddBooking(thirdBooking);
        _bookingRepository.AddBooking(fourthBooking);

        await _bookingRepository.SaveChangesAsync();

        var startTime = new DateTime(2004, 8, 17, 10, 0, 0);
        var endTime = new DateTime(2004, 8, 17, 10, 59, 0);

        var availableConveners = await _bookingRepository.GetBookingsForTimeRangeAsync(startTime, endTime);
        Assert.Equal(4, availableConveners.Count());
    }

    [Fact]
    public async void Given_ValidStartAndEndTime_ForEdgeCase1_When_GetBookingsForTimeRangeAsyncIsCalled_Then_LessThan4IsReturned()
    {
        var firstBooking = new Booking(new DateTime(2004, 8, 17, 14, 0, 0), new DateTime(2004, 8, 17, 14, 59, 0), "John Smith", Convener.FIRST_CONVENER);
        var secondBooking = new Booking(new DateTime(2004, 8, 17, 14, 0, 0), new DateTime(2004, 8, 17, 14, 59, 0), "John Smith", Convener.SECOND_CONVENER);
        var thirdBooking = new Booking(new DateTime(2004, 8, 17, 14, 45, 0), new DateTime(2004, 8, 17, 15, 44, 0), "John Smith", Convener.THIRD_CONVENER);
        var fourthBooking = new Booking(new DateTime(2004, 8, 17, 15, 0, 0), new DateTime(2004, 8, 17, 15, 59, 0), "John Smith", Convener.FIRST_CONVENER);
        _bookingRepository.AddBooking(firstBooking);
        _bookingRepository.AddBooking(secondBooking);
        _bookingRepository.AddBooking(thirdBooking);
        _bookingRepository.AddBooking(fourthBooking);

        await _bookingRepository.SaveChangesAsync();

        var startTime = new DateTime(2004, 8, 17, 14, 30, 0);
        var endTime = new DateTime(2004, 8, 17, 15, 29, 0);

        var availableConveners = await _bookingRepository.GetBookingsForTimeRangeAsync(startTime, endTime);
        Assert.Equal(4, availableConveners.Count());
    }

}

