using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SportsBookingSystem.Models;

[Table("Inquiry")]
public partial class Inquiry
{
    [Key]
    [Column("InquiryID")]
    public int InquiryId { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? GuestName { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Email { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Message { get; set; }

    public DateOnly? DateSent { get; set; }
}
