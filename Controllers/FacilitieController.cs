
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Models;
using SportsBookingSystem.Data;

public class FacilitieController : Controller
{
    private readonly SportsContext _context;

    public FacilitieController(SportsContext context)
    {
        _context = context;
    }

    // GET: FACILITYS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Facilities.ToListAsync());
    }

    // GET: FACILITYS/Details/5
    public async Task<IActionResult> Details(int? facilityid)
    {
        if (facilityid == null)
        {
            return NotFound();
        }

        var facility = await _context.Facilities
            .FirstOrDefaultAsync(m => m.FacilityId == facilityid);
        if (facility == null)
        {
            return NotFound();
        }

        return View(facility);
    }

    // GET: FACILITYS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: FACILITYS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("FacilityId,Name,TypeId,Location,Capacity,HourlyRate,Bookings,Reviews,Type")] Facility facility)
    {
        if (ModelState.IsValid)
        {
            _context.Add(facility);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(facility);
    }

    // GET: FACILITYS/Edit/5
    public async Task<IActionResult> Edit(int? facilityid)
    {
        if (facilityid == null)
        {
            return NotFound();
        }

        var facility = await _context.Facilities.FindAsync(facilityid);
        if (facility == null)
        {
            return NotFound();
        }
        return View(facility);
    }

    // POST: FACILITYS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? facilityid, [Bind("FacilityId,Name,TypeId,Location,Capacity,HourlyRate,Bookings,Reviews,Type")] Facility facility)
    {
        if (facilityid != facility.FacilityId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(facility);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FacilityExists(facility.FacilityId))
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
        return View(facility);
    }

    // GET: FACILITYS/Delete/5
    public async Task<IActionResult> Delete(int? facilityid)
    {
        if (facilityid == null)
        {
            return NotFound();
        }

        var facility = await _context.Facilities
            .FirstOrDefaultAsync(m => m.FacilityId == facilityid);
        if (facility == null)
        {
            return NotFound();
        }

        return View(facility);
    }

    // POST: FACILITYS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? facilityid)
    {
        var facility = await _context.Facilities.FindAsync(facilityid);
        if (facility != null)
        {
            _context.Facilities.Remove(facility);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool FacilityExists(int? facilityid)
    {
        return _context.Facilities.Any(e => e.FacilityId == facilityid);
    }
}
