using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Data;
using SportsBookingSystem.Models;

namespace SportsBookingSystem.Controllers;

public class FacilityController : Controller
{
    private readonly SportsContext _context;
    public FacilityController(SportsContext context) { _context = context; }

    public IActionResult Index() => RedirectToAction(nameof(Search));

    public async Task<IActionResult> Search([Bind("TypeId,Location,Date,StartTime,EndTime")] FacilitySearchViewModel model)
    {
        ViewBag.Types = new SelectList(await _context.FacilityTypes.OrderBy(t => t.TypeName).ToListAsync(), "TypeId", "TypeName");
        if (model.StartTime.HasValue != model.EndTime.HasValue)
            ModelState.AddModelError("", "Enter both a start and end time.");
        if (model.StartTime.HasValue && !model.Date.HasValue)
            ModelState.AddModelError("", "Choose a date when searching by time.");
        if (model.StartTime.HasValue && model.EndTime <= model.StartTime)
            ModelState.AddModelError("", "End time must be after start time.");
        if (!ModelState.IsValid) return View(model);

        var facilities = _context.Facilities.Include(f => f.Type).AsQueryable();
        if (model.TypeId.HasValue) facilities = facilities.Where(f => f.TypeId == model.TypeId);
        if (!string.IsNullOrWhiteSpace(model.Location))
            facilities = facilities.Where(f => f.Location != null && f.Location.Contains(model.Location.Trim()));
        if (model.Date.HasValue)
        {
            var start = model.Date.Value.ToDateTime(model.StartTime ?? TimeOnly.MinValue);
            var end = model.EndTime.HasValue ? model.Date.Value.ToDateTime(model.EndTime.Value) : start.AddDays(1);
            facilities = facilities.Where(f => !f.Bookings.Any(b => b.Status != "Cancelled" && b.BookingDate == model.Date
                && (b.StartTime == null || b.EndTime == null || (b.StartTime < end && b.EndTime > start))));
        }
        model.Facilities = await facilities.OrderBy(f => f.Name).ToListAsync();
        return View(model);
    }
}
