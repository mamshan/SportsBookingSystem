using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SportsBookingSystem.Models;

[Table("FacilityType")]
public partial class FacilityType
{
    [Key]
    [Column("TypeID")]
    public int TypeId { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string TypeName { get; set; } = null!;

    [InverseProperty("Type")]
    public virtual ICollection<Facility> Facilities { get; set; } = new List<Facility>();

    [InverseProperty("Type")]
    public virtual ICollection<SportPreference> SportPreferences { get; set; } = new List<SportPreference>();
}
