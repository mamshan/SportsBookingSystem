
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Models;
using SportsBookingSystem.Data;

public class BookingController : Controller
{
    private readonly SportsContext _context;

    public BookingController(SportsContext context)
    {
        _context = context;
    }

    // GET: BOOKINGS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Bookings.ToListAsync());
    }

    // GET: BOOKINGS/Details/5
    public async Task<IActionResult> Details(int? bookingid)
    {
        if (bookingid == null)
        {
            return NotFound();
        }

        var booking = await _context.Bookings
            .FirstOrDefaultAsync(m => m.BookingId == bookingid);
        if (booking == null)
        {
            return NotFound();
        }

        return View(booking);
    }

    // GET: BOOKINGS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: BOOKINGS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("BookingId,MemberId,FacilityId,BookingDate,StartTime,EndTime,Status,Facility,Member")] Booking booking)
    {
        if (ModelState.IsValid)
        {
            _context.Add(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(booking);
    }

    // GET: BOOKINGS/Edit/5
    public async Task<IActionResult> Edit(int? bookingid)
    {
        if (bookingid == null)
        {
            return NotFound();
        }

        var booking = await _context.Bookings.FindAsync(bookingid);
        if (booking == null)
        {
            return NotFound();
        }
        return View(booking);
    }

    // POST: BOOKINGS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? bookingid, [Bind("BookingId,MemberId,FacilityId,BookingDate,StartTime,EndTime,Status,Facility,Member")] Booking booking)
    {
        if (bookingid != booking.BookingId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(booking);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(booking.BookingId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(booking);
    }

    // GET: BOOKINGS/Delete/5
    public async Task<IActionResult> Delete(int? bookingid)
    {
        if (bookingid == null)
        {
            return NotFound();
        }

        var booking = await _context.Bookings
            .FirstOrDefaultAsync(m => m.BookingId == bookingid);
        if (booking == null)
        {
            return NotFound();
        }

        return View(booking);
    }

    // POST: BOOKINGS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? bookingid)
    {
        var booking = await _context.Bookings.FindAsync(bookingid);
        if (booking != null)
        {
            _context.Bookings.Remove(booking);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool BookingExists(int? bookingid)
    {
        return _context.Bookings.Any(e => e.BookingId == bookingid);
    }
}
