using System.ComponentModel.DataAnnotations;

namespace SportsBookingSystem.Models;

public class ReviewViewModel
{
    [Range(1, int.MaxValue), Display(Name = "Facility")]
    public int FacilityId { get; set; }
    [Range(1, 5)]
    public int Rating { get; set; } = 5;
    [StringLength(255)]
    public string? Comments { get; set; }
}
