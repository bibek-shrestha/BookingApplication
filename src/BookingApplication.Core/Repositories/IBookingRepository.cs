using BookingApplication.Core.Entities;

namespace BookingApplication.Core.Repositories;

public interface IBookingRepository
{
    void AddBooking(Booking booking);

    Task<IEnumerable<Booking>> GetBookingsForTimeRangeAsync(DateTime startTime, DateTime endTime);

    Task<int> SaveChangesAsync();
}
