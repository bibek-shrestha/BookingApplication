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
    public void AddBooking_ShouldStartTrackingTheBooking()
    {
        var booking = BookingHelper.CreateNewBooking();
        _bookingRepository.AddBooking(booking);
        var trackedBooking = _context.ChangeTracker.Entries<Booking>().FirstOrDefault();
        Assert.NotNull(trackedBooking);
        Assert.Equal(EntityState.Added, trackedBooking.State);
    }

    [Fact]
    public async void SaveChangesAsync_ShouldSaveTheBookingEntity_AndUpdateTrackingInformation()
    {
        var booking = BookingHelper.CreateNewBooking();
        _bookingRepository.AddBooking(booking);
        var numberOfSavedRecords = await _bookingRepository.SaveChangesAsync();
        var trackedBooking = _context.ChangeTracker.Entries<Booking>().FirstOrDefault();
        Assert.Equal(1, numberOfSavedRecords);
        Assert.NotNull(trackedBooking);
        Assert.Equal(EntityState.Unchanged, trackedBooking.State);
    }

}

