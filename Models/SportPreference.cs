using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SportsBookingSystem.Models;

[Table("SportPreference")]
[Index("MemberId", "TypeId", Name = "SportPreference__UN", IsUnique = true)]
public partial class SportPreference
{
    [Key]
    [Column("SportPreferenceID")]
    public int SportPreferenceId { get; set; }

    [Column("MemberID")]
    public int MemberId { get; set; }

    [Column("TypeID")]
    public int TypeId { get; set; }

    [ForeignKey("MemberId")]
    [InverseProperty("SportPreferences")]
    public virtual Member Member { get; set; } = null!;

    [ForeignKey("TypeId")]
    [InverseProperty("SportPreferences")]
    public virtual FacilityType Type { get; set; } = null!;
}
