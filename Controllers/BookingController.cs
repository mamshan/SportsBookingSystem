using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Data;
using SportsBookingSystem.Models;

namespace SportsBookingSystem.Controllers;

public class BookingController : Controller
{
    private readonly SportsContext _context;
    public BookingController(SportsContext context) { _context = context; }

    public IActionResult Index() => RedirectToAction(nameof(MyBookings));

    public async Task<IActionResult> MyBookings()
    {
        var memberId = HttpContext.Session.GetInt32("MemberID");
        if (memberId == null) return RedirectToAction("Login", "Account");
        return View(await _context.Bookings.Include(b => b.Facility).Where(b => b.MemberId == memberId)
            .OrderByDescending(b => b.BookingDate).ThenBy(b => b.StartTime).ToListAsync());
    }

    public async Task<IActionResult> Create(int? facilityId)
    {
        if (HttpContext.Session.GetInt32("MemberID") == null) return RedirectToAction("Login", "Account");
        await LoadFacilities();
        return View(new BookingViewModel { FacilityId = facilityId ?? 0 });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookingViewModel model)
    {
        var memberId = HttpContext.Session.GetInt32("MemberID");
        if (memberId == null) return RedirectToAction("Login", "Account");
        if (!await _context.Facilities.AnyAsync(f => f.FacilityId == model.FacilityId))
            ModelState.AddModelError("FacilityId", "Choose an existing facility.");
        if (model.StartTime.HasValue && model.EndTime <= model.StartTime)
            ModelState.AddModelError("EndTime", "End time must be after start time.");
        if (model.BookingDate.HasValue && model.StartTime.HasValue && model.BookingDate.Value.ToDateTime(model.StartTime.Value) <= DateTime.Now)
            ModelState.AddModelError("BookingDate", "Choose a future date and time.");
        if (ModelState.IsValid)
        {
            var start = model.BookingDate!.Value.ToDateTime(model.StartTime!.Value);
            var end = model.BookingDate.Value.ToDateTime(model.EndTime!.Value);
            // Keep the availability check and booking together.
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var clash = await _context.Bookings.AnyAsync(b => b.FacilityId == model.FacilityId
                && b.BookingDate == model.BookingDate && b.Status != "Cancelled"
                && (b.StartTime == null || b.EndTime == null || (b.StartTime < end && b.EndTime > start)));
            if (clash) ModelState.AddModelError("", "This facility is already booked during that time.");
            else
            {
                _context.Bookings.Add(new Booking { MemberId = memberId.Value, FacilityId = model.FacilityId,
                    BookingDate = model.BookingDate.Value, StartTime = start, EndTime = end, Status = "Pending" });
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["Success"] = "Your booking request has been saved.";
                return RedirectToAction(nameof(MyBookings));
            }
        }
        await LoadFacilities();
        return View(model);
    }

    private async Task LoadFacilities()
    {
        ViewBag.Facilities = new SelectList(await _context.Facilities.OrderBy(f => f.Name).ToListAsync(), "FacilityId", "Name");
    }
}
