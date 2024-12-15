namespace BookingApplication.Infrastructure.Models;

public class BookingCreationDto
{
    public string BookingTime { get; set; }

    public string Name { get; set; } = string.Empty;

    public BookingCreationDto(string bookingTime, string name)
    {
        BookingTime = bookingTime;
        Name = name;
    }
}
