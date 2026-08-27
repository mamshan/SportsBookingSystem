using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SportsBookingSystem.Models;

[Table("Facility")]
public partial class Facility
{
    [Key]
    [Column("FacilityID")]
    public int FacilityId { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Column("TypeID")]
    public int TypeId { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Location { get; set; }

    public int? Capacity { get; set; }

    [Column(TypeName = "decimal(25, 2)")]
    public decimal? HourlyRate { get; set; }

    [InverseProperty("Facility")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [InverseProperty("Facility")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [ForeignKey("TypeId")]
    [InverseProperty("Facilities")]
    public virtual FacilityType Type { get; set; } = null!;
}
