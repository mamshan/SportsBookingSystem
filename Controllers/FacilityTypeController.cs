
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Models;
using SportsBookingSystem.Data;

public class FacilityTypeController : Controller
{
    private readonly SportsContext _context;

    public FacilityTypeController(SportsContext context)
    {
        _context = context;
    }

    // GET: FACILITYTYPES
    public async Task<IActionResult> Index()    
    {
        return View(await _context.FacilityTypes.ToListAsync());
    }

    // GET: FACILITYTYPES/Details/5
    public async Task<IActionResult> Details(int? typeid)
    {
        if (typeid == null)
        {
            return NotFound();
        }

        var facilitytype = await _context.FacilityTypes
            .FirstOrDefaultAsync(m => m.TypeId == typeid);
        if (facilitytype == null)
        {
            return NotFound();
        }

        return View(facilitytype);
    }

    // GET: FACILITYTYPES/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: FACILITYTYPES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("TypeId,TypeName,Facilities,SportPreferences")] FacilityType facilitytype)
    {
        if (ModelState.IsValid)
        {
            _context.Add(facilitytype);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(facilitytype);
    }

    // GET: FACILITYTYPES/Edit/5
    public async Task<IActionResult> Edit(int? typeid)
    {
        if (typeid == null)
        {
            return NotFound();
        }

        var facilitytype = await _context.FacilityTypes.FindAsync(typeid);
        if (facilitytype == null)
        {
            return NotFound();
        }
        return View(facilitytype);
    }

    // POST: FACILITYTYPES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? typeid, [Bind("TypeId,TypeName,Facilities,SportPreferences")] FacilityType facilitytype)
    {
        if (typeid != facilitytype.TypeId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(facilitytype);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FacilityTypeExists(facilitytype.TypeId))
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
        return View(facilitytype);
    }

    // GET: FACILITYTYPES/Delete/5
    public async Task<IActionResult> Delete(int? typeid)
    {
        if (typeid == null)
        {
            return NotFound();
        }

        var facilitytype = await _context.FacilityTypes
            .FirstOrDefaultAsync(m => m.TypeId == typeid);
        if (facilitytype == null)
        {
            return NotFound();
        }

        return View(facilitytype);
    }

    // POST: FACILITYTYPES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? typeid)
    {
        var facilitytype = await _context.FacilityTypes.FindAsync(typeid);
        if (facilitytype != null)
        {
            _context.FacilityTypes.Remove(facilitytype);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool FacilityTypeExists(int? typeid)
    {
        return _context.FacilityTypes.Any(e => e.TypeId == typeid);
    }
}
