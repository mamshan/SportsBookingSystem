using System.ComponentModel.DataAnnotations;

namespace SportsBookingSystem.Models;

public class FacilitySearchViewModel
{
    [Display(Name = "Sport")]
    public int? TypeId { get; set; }
    public string? Location { get; set; }
    [DataType(DataType.Date)]
    public DateOnly? Date { get; set; }
    [DataType(DataType.Time), Display(Name = "Start time")]
    public TimeOnly? StartTime { get; set; }
    [DataType(DataType.Time), Display(Name = "End time")]
    public TimeOnly? EndTime { get; set; }
    public List<Facility> Facilities { get; set; } = new();
}
