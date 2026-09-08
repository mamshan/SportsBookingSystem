using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Data;
using SportsBookingSystem.Models;

namespace SportsBookingSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly SportsContext _context;

        public AccountController(SportsContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Please enter your email and password.");
                return View();
            }

            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.Email == email && m.Password == password);

            if (member == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View();
            }

            HttpContext.Session.SetInt32("MemberID", member.MemberId);
            HttpContext.Session.SetString("MemberName", member.Name);

            return RedirectToAction("Search", "Facility");
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            ViewBag.Sports = await _context.FacilityTypes.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (await _context.Members.AnyAsync(m => m.Email == model.Email))
            {
                ModelState.AddModelError("Email", "That email address is already registered.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Sports = await _context.FacilityTypes.ToListAsync();
                return View(model);
            }

            var member = new Member
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                Password = model.Password,
                RegDate = DateOnly.FromDateTime(DateTime.Now)
            };

            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            foreach (var typeId in model.SelectedSports)
            {
                _context.SportPreferences.Add(new SportPreference
                {
                    MemberId = member.MemberId,
                    TypeId = typeId
                });
            }
            await _context.SaveChangesAsync();

            HttpContext.Session.SetInt32("MemberID", member.MemberId);
            HttpContext.Session.SetString("MemberName", member.Name);

            return RedirectToAction("Search", "Facility");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
