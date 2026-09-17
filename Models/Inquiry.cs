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
    [Required, Display(Name = "Your name")]
    public string? GuestName { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    [Required, EmailAddress]
    public string? Email { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    [Required]
    public string? Message { get; set; }

    public DateOnly? DateSent { get; set; }
}
