using Microsoft.AspNetCore.Mvc;
using SportsBookingSystem.Data;
using SportsBookingSystem.Models;

namespace SportsBookingSystem.Controllers;

public class InquiryController : Controller
{
    private readonly SportsContext _context;
    public InquiryController(SportsContext context) { _context = context; }

    public IActionResult Create() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("GuestName,Email,Message")] Inquiry inquiry)
    {
        if (!ModelState.IsValid) return View(inquiry);
        inquiry.DateSent = DateOnly.FromDateTime(DateTime.Today);
        _context.Inquiries.Add(inquiry);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Your inquiry has been sent to the Sports Council.";
        return RedirectToAction(nameof(Create));
    }
}
