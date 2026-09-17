using System.ComponentModel.DataAnnotations;

namespace SportsBookingSystem.Models;

public class BookingViewModel
{
    [Range(1, int.MaxValue), Display(Name = "Facility")]
    public int FacilityId { get; set; }
    [Required, DataType(DataType.Date), Display(Name = "Date")]
    public DateOnly? BookingDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    [Required, DataType(DataType.Time), Display(Name = "Start time")]
    public TimeOnly? StartTime { get; set; }
    [Required, DataType(DataType.Time), Display(Name = "End time")]
    public TimeOnly? EndTime { get; set; }
}
