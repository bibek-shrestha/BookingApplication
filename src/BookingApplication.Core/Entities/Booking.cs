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

    [Required]
    public Convener Convener {get; set; }

    public Booking(DateTime startTime, DateTime endTime, string name, Convener convener)
    {
        Id = Guid.NewGuid();
        StartTime = startTime;
        EndTime = endTime;
        Name = name;
        Convener = convener;
    }

}
