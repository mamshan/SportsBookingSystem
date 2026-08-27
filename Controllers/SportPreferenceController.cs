
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Models;
using SportsBookingSystem.Data;

public class SportPreferenceController : Controller
{
    private readonly SportsContext _context;

    public SportPreferenceController(SportsContext context)
    {
        _context = context;
    }

    // GET: SPORTPREFERENCES
    public async Task<IActionResult> Index()    
    {
        return View(await _context.SportPreferences.ToListAsync());
    }

    // GET: SPORTPREFERENCES/Details/5
    public async Task<IActionResult> Details(int? sportpreferenceid)
    {
        if (sportpreferenceid == null)
        {
            return NotFound();
        }

        var sportpreference = await _context.SportPreferences
            .FirstOrDefaultAsync(m => m.SportPreferenceId == sportpreferenceid);
        if (sportpreference == null)
        {
            return NotFound();
        }

        return View(sportpreference);
    }

    // GET: SPORTPREFERENCES/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: SPORTPREFERENCES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("SportPreferenceId,MemberId,TypeId,Member,Type")] SportPreference sportpreference)
    {
        if (ModelState.IsValid)
        {
            _context.Add(sportpreference);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(sportpreference);
    }

    // GET: SPORTPREFERENCES/Edit/5
    public async Task<IActionResult> Edit(int? sportpreferenceid)
    {
        if (sportpreferenceid == null)
        {
            return NotFound();
        }

        var sportpreference = await _context.SportPreferences.FindAsync(sportpreferenceid);
        if (sportpreference == null)
        {
            return NotFound();
        }
        return View(sportpreference);
    }

    // POST: SPORTPREFERENCES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? sportpreferenceid, [Bind("SportPreferenceId,MemberId,TypeId,Member,Type")] SportPreference sportpreference)
    {
        if (sportpreferenceid != sportpreference.SportPreferenceId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(sportpreference);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SportPreferenceExists(sportpreference.SportPreferenceId))
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
        return View(sportpreference);
    }

    // GET: SPORTPREFERENCES/Delete/5
    public async Task<IActionResult> Delete(int? sportpreferenceid)
    {
        if (sportpreferenceid == null)
        {
            return NotFound();
        }

        var sportpreference = await _context.SportPreferences
            .FirstOrDefaultAsync(m => m.SportPreferenceId == sportpreferenceid);
        if (sportpreference == null)
        {
            return NotFound();
        }

        return View(sportpreference);
    }

    // POST: SPORTPREFERENCES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? sportpreferenceid)
    {
        var sportpreference = await _context.SportPreferences.FindAsync(sportpreferenceid);
        if (sportpreference != null)
        {
            _context.SportPreferences.Remove(sportpreference);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool SportPreferenceExists(int? sportpreferenceid)
    {
        return _context.SportPreferences.Any(e => e.SportPreferenceId == sportpreferenceid);
    }
}
