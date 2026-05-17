using Microsoft.AspNetCore.Mvc;
using BandMate.Models;
using BCrypt.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace BandMate.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // KAYIT OL (GET)
        public IActionResult Register() => View();

        // KAYIT OL (POST)
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            // Basitçe "User" rolünü ata (SeedData'dan Id'sini bulabilirsin, genelde 2'dir)
            var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            user.RoleId = userRole.Id;

            // Şifreyi hash'le
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Login");
        }

        // GİRİŞ YAP (GET)
        public IActionResult Login() => View();

        // GİRİŞ YAP (POST)
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.Users.Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                // Kimlik kartını (Claims) oluşturuyoruz
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.Name),
                    new Claim("UserId", user.Id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "E-posta veya şifre hatalı!";
            return View();
        }

        // ÇIKIŞ YAP
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}