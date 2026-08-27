using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SportsBookingSystem.Models;

[Table("Review")]
public partial class Review
{
    [Key]
    [Column("ReviewID")]
    public int ReviewId { get; set; }

    [Column("MemberID")]
    public int MemberId { get; set; }

    [Column("FacilityID")]
    public int FacilityId { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string Rating { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string? Comments { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ReviewDate { get; set; }

    [ForeignKey("FacilityId")]
    [InverseProperty("Reviews")]
    public virtual Facility Facility { get; set; } = null!;

    [ForeignKey("MemberId")]
    [InverseProperty("Reviews")]
    public virtual Member Member { get; set; } = null!;
}
