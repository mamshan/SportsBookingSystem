using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Data;
using SportsBookingSystem.Models;

namespace SportsBookingSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly SportsContext _context;
        private readonly PasswordHasher<Member> _passwordHasher = new();

        public AccountController(SportsContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(Login));
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
                .FirstOrDefaultAsync(m => m.Email == email.Trim());

            if (member == null) {
                ModelState.AddModelError("", "Invalid email.");
                return View(); 
            }

            var passwordIsValid = member != null &&
                _passwordHasher.VerifyHashedPassword(member, member.Password, password) != PasswordVerificationResult.Failed;

            if (!passwordIsValid && member != null && member.Password == password)
            {
                member.Password = _passwordHasher.HashPassword(member, password);
                passwordIsValid = true;
            }

            if (!passwordIsValid)
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
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            model.Email = model.Email?.Trim() ?? "";
            model.Name = model.Name?.Trim() ?? "";
            model.SelectedSports = (model.SelectedSports ?? new List<int>()).Distinct().ToList();
            var sports = await _context.FacilityTypes.OrderBy(t => t.TypeName).ToListAsync();
            if (model.SelectedSports.Count == 0)
                ModelState.AddModelError("SelectedSports", "Choose at least one preferred sport.");
            if (model.SelectedSports.Any(id => !sports.Any(s => s.TypeId == id)))
                ModelState.AddModelError("SelectedSports", "Choose a sport from the list.");
            if (await _context.Members.AnyAsync(m => m.Email == model.Email))
            {
                ModelState.AddModelError("Email", "That email address is already registered.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Sports = sports;
                return View(model);
            }

            var member = new Member
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                Password = _passwordHasher.HashPassword(null!, model.Password),
                RegDate = DateOnly.FromDateTime(DateTime.Now)
            };

            foreach (var typeId in model.SelectedSports)
            {
                member.SportPreferences.Add(new SportPreference
                {
                    TypeId = typeId
                });
            }
            _context.Members.Add(member);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Registration could not be saved. Check your details and try again.");
                ViewBag.Sports = sports;
                return View(model);
            }

            HttpContext.Session.SetInt32("MemberID", member.MemberId);
            HttpContext.Session.SetString("MemberName", member.Name);

            return RedirectToAction("Search", "Facility");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
