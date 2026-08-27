using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SportsBookingSystem.Models;

[Table("Booking")]
public partial class Booking
{
    [Key]
    [Column("BookingID")]
    public int BookingId { get; set; }

    [Column("MemberID")]
    public int MemberId { get; set; }

    [Column("FacilityID")]
    public int FacilityId { get; set; }

    public DateOnly BookingDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartTime { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndTime { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string Status { get; set; } = null!;

    [ForeignKey("FacilityId")]
    [InverseProperty("Bookings")]
    public virtual Facility Facility { get; set; } = null!;

    [ForeignKey("MemberId")]
    [InverseProperty("Bookings")]
    public virtual Member Member { get; set; } = null!;
}
