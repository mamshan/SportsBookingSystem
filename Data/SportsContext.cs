using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Models;

namespace SportsBookingSystem.Data;

public partial class SportsContext : DbContext
{
    public SportsContext(DbContextOptions<SportsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Facility> Facilities { get; set; }

    public virtual DbSet<FacilityType> FacilityTypes { get; set; }

    public virtual DbSet<Inquiry> Inquiries { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<SportPreference> SportPreferences { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("Booking_PK");

            entity.Property(e => e.BookingId).ValueGeneratedOnAdd();
            entity.Property(e => e.MemberId).ValueGeneratedNever();

            entity.HasOne(d => d.Facility).WithMany(p => p.Bookings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Booking_Facility_FK");

            entity.HasOne(d => d.Member).WithMany(p => p.Bookings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Booking_Member_FK");
        });

        modelBuilder.Entity<Facility>(entity =>
        {
            entity.HasKey(e => e.FacilityId).HasName("Facility_PK");

            entity.HasOne(d => d.Type).WithMany(p => p.Facilities)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Facility_FacilityType_FK");
        });

        modelBuilder.Entity<FacilityType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("FacilityType_PK");
        });

        modelBuilder.Entity<Inquiry>(entity =>
        {
            entity.HasKey(e => e.InquiryId).HasName("Inquiry_PK");
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.MemberId).HasName("Member_PK");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("Review_PK");

            entity.HasOne(d => d.Facility).WithMany(p => p.Reviews)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Review_Facility_FK");

            entity.HasOne(d => d.Member).WithMany(p => p.Reviews)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Review_Member_FK");
        });

        modelBuilder.Entity<SportPreference>(entity =>
        {
            entity.HasKey(e => e.SportPreferenceId).HasName("SportPreference_PK");

            entity.HasOne(d => d.Member).WithMany(p => p.SportPreferences)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SportPreference_Member_FK");

            entity.HasOne(d => d.Type).WithMany(p => p.SportPreferences)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SportPreference_FacilityType_FK");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
