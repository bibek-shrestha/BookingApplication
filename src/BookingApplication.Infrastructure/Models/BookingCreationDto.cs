using System;
using System.ComponentModel.DataAnnotations;

namespace BookingApplication.Infrastructure.Models;

public class BookingCreationDto
{
    [Required]
    [Timestamp]
    public string BookingTime { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public BookingCreationDto(string bookingTime, string name)
    {
        BookingTime = bookingTime;
        Name = name;
    }
}
