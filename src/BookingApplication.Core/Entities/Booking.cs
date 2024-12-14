using System.ComponentModel.DataAnnotations;

namespace BookingApplication.Core.Entities;

public class Booking
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public DateTime StartTime { get; set; }
    [Required]
    public DateTime EndTime { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;

    public Booking(DateTime startTime, DateTime endTime, string name)
    {
        Id = Guid.NewGuid();
        StartTime = startTime;
        EndTime = endTime;
        Name = name;
    }

}
