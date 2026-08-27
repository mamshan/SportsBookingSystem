
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Models;
using SportsBookingSystem.Data;

public class InquiryController : Controller
{
    private readonly SportsContext _context;

    public InquiryController(SportsContext context)
    {
        _context = context;
    }

    // GET: INQUIRYS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Inquiries.ToListAsync());
    }

    // GET: INQUIRYS/Details/5
    public async Task<IActionResult> Details(int? inquiryid)
    {
        if (inquiryid == null)
        {
            return NotFound();
        }

        var inquiry = await _context.Inquiries
            .FirstOrDefaultAsync(m => m.InquiryId == inquiryid);
        if (inquiry == null)
        {
            return NotFound();
        }

        return View(inquiry);
    }

    // GET: INQUIRYS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: INQUIRYS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("InquiryId,GuestName,Email,Message,DateSent")] Inquiry inquiry)
    {
        if (ModelState.IsValid)
        {
            _context.Add(inquiry);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(inquiry);
    }

    // GET: INQUIRYS/Edit/5
    public async Task<IActionResult> Edit(int? inquiryid)
    {
        if (inquiryid == null)
        {
            return NotFound();
        }

        var inquiry = await _context.Inquiries.FindAsync(inquiryid);
        if (inquiry == null)
        {
            return NotFound();
        }
        return View(inquiry);
    }

    // POST: INQUIRYS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? inquiryid, [Bind("InquiryId,GuestName,Email,Message,DateSent")] Inquiry inquiry)
    {
        if (inquiryid != inquiry.InquiryId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(inquiry);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InquiryExists(inquiry.InquiryId))
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
        return View(inquiry);
    }

    // GET: INQUIRYS/Delete/5
    public async Task<IActionResult> Delete(int? inquiryid)
    {
        if (inquiryid == null)
        {
            return NotFound();
        }

        var inquiry = await _context.Inquiries
            .FirstOrDefaultAsync(m => m.InquiryId == inquiryid);
        if (inquiry == null)
        {
            return NotFound();
        }

        return View(inquiry);
    }

    // POST: INQUIRYS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? inquiryid)
    {
        var inquiry = await _context.Inquiries.FindAsync(inquiryid);
        if (inquiry != null)
        {
            _context.Inquiries.Remove(inquiry);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool InquiryExists(int? inquiryid)
    {
        return _context.Inquiries.Any(e => e.InquiryId == inquiryid);
    }
}
