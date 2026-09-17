using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Data;
using SportsBookingSystem.Models;

namespace SportsBookingSystem.Controllers;

public class ReviewController : Controller
{
    private readonly SportsContext _context;
    public ReviewController(SportsContext context) { _context = context; }

    public async Task<IActionResult> Index(int? facilityId, int? rating)
    {
        ViewBag.Facilities = new SelectList(await _context.Facilities.OrderBy(f => f.Name).ToListAsync(), "FacilityId", "Name", facilityId);
        ViewBag.Rating = rating;
        var reviews = _context.Reviews.Include(r => r.Facility).Include(r => r.Member).AsQueryable();
        if (facilityId.HasValue) reviews = reviews.Where(r => r.FacilityId == facilityId);
        if (rating.HasValue) reviews = reviews.Where(r => r.Rating == rating.Value);
        return View(await reviews.OrderByDescending(r => r.ReviewDate).ToListAsync());
    }

    public async Task<IActionResult> Create()
    {
        var memberId = HttpContext.Session.GetInt32("MemberID");
        if (memberId == null) return RedirectToAction("Login", "Account");
        await LoadFacilities(memberId.Value);
        return View(new ReviewViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReviewViewModel model)
    {
        var memberId = HttpContext.Session.GetInt32("MemberID");
        if (memberId == null) return RedirectToAction("Login", "Account");
        if (!await _context.Bookings.AnyAsync(b => b.MemberId == memberId && b.FacilityId == model.FacilityId
            && b.Status == "Confirmed" && b.EndTime < DateTime.Now))
            ModelState.AddModelError("FacilityId", "You can review a facility after a confirmed booking has ended.");
        if (ModelState.IsValid)
        {
            _context.Reviews.Add(new Review { MemberId = memberId.Value, FacilityId = model.FacilityId,
                Rating = model.Rating, Comments = model.Comments, ReviewDate = DateTime.Now });
            await _context.SaveChangesAsync();
            TempData["Success"] = "Thank you. Your review has been saved.";
            return RedirectToAction(nameof(Index));
        }
        await LoadFacilities(memberId.Value);
        return View(model);
    }

    private async Task LoadFacilities(int memberId)
    {
        var facilities = await _context.Facilities.Where(f => f.Bookings.Any(b => b.MemberId == memberId
            && b.Status == "Confirmed" && b.EndTime < DateTime.Now)).OrderBy(f => f.Name).ToListAsync();
        ViewBag.Facilities = new SelectList(facilities, "FacilityId", "Name");
    }
}
