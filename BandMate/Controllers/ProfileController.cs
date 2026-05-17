using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BandMate.Models;
using System.Security.Claims;

namespace BandMate.Controllers
{
    [Authorize] // Sadece giriş yapan üyeler paneli görebilir
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        // KULLANICI PANELİ ANA SAYFASI
        public async Task<IActionResult> Index()
        {
            // 1. Giriş yapan kullanıcının ID'sini alıyoruz
            var userIdString = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("Login", "Account");

            int currentUserId = int.Parse(userIdString);

            // 2. Kullanıcının kendi açtığı ilanları çekiyoruz
            var myListings = await _context.Listings
                .Include(l => l.Category)
                .Where(l => l.UserId == currentUserId)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

            // 3. Kullanıcının ilanlarına GELEN başvuruları çekiyoruz
            var receivedApplications = await _context.Applications
                .Include(a => a.Listing) // Hangi ilana gelmiş?
                .Include(a => a.User)    // Başvuruyu kim yapmış?
                .Where(a => a.Listing.UserId == currentUserId) // İlanın sahibi bendeysem
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            // 4. ViewModel'e verileri doldurup View'a gönderiyoruz
            var viewModel = new ProfileViewModel
            {
                MyListings = myListings,
                ReceivedApplications = receivedApplications
            };

            return View(viewModel);
        }

        // İLANI AKTİF/PASİF YAPMA (Hızlı Yönetim)
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var listing = await _context.Listings.FindAsync(id);
            var userId = int.Parse(User.FindFirstValue("UserId"));

            // Güvenlik Kontrolü: İlan gerçekten bu kullanıcıya mı ait?
            if (listing != null && listing.UserId == userId)
            {
                listing.IsActive = !listing.IsActive; // Aktifse pasif, pasifse aktif yapar
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}