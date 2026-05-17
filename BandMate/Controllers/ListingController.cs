using BandMate.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BandMate.Controllers
{
    [Authorize] // Sadece giriş yapanlar erişebilir
    public class ListingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ListingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // İLANLARI LİSTELE (TÜMÜ)
        // AMA BURASI İSTİSNA: İlanları herkes görebilir.
        [AllowAnonymous]
        public async Task<IActionResult> Index(string searchInstrument, string searchCity)
        {
            // 1. Temel Sorgu: Aktif ilanları, kategorisi ve kullanıcısıyla birlikte çekmeye hazırla
            var listingsQuery = _context.Listings
                .Include(l => l.Category)
                .Include(l => l.User)
                .Where(l => l.IsActive);

            // 2. Eğer Enstrüman kutusu doldurulduysa sorguya EKLE (Büyük/Küçük harf duyarsız arar)
            if (!string.IsNullOrEmpty(searchInstrument))
            {
                listingsQuery = listingsQuery.Where(l => l.Instrument.Contains(searchInstrument));
            }

            // 3. Eğer Şehir kutusu doldurulduysa sorguya EKLE
            if (!string.IsNullOrEmpty(searchCity))
            {
                listingsQuery = listingsQuery.Where(l => l.City.Contains(searchCity));
            }

            // Arama kutularında yazılan değerler sayfa yenilenince kaybolmasın diye ViewBag ile geri gönderiyoruz
            ViewBag.CurrentInstrument = searchInstrument;
            ViewBag.CurrentCity = searchCity;

            // 4. Filtrelenmiş sorguyu veritabanından çek ve tarihe göre sırala
            var filteredListings = await listingsQuery.OrderByDescending(l => l.CreatedAt).ToListAsync();

            return View(filteredListings);
        }

        // BURASI DA İSTİSNA: İlan detayına herkes bakabilir.
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            // 'id' parametresinin burada olduğuna emin ol!
            var listing = await _context.Listings
                .Include(l => l.Category)
                .Include(l => l.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (listing == null) return NotFound();

            return View(listing);
        }

        // YENİ İLAN OLUŞTUR (GET)
        public IActionResult Create()
        {
            // Dropdown listesi için kategorileri çekiyoruz
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // YENİ İLAN OLUŞTUR (POST)
        [HttpPost]
        public async Task<IActionResult> Create(Listing listing)
        {
            // Giriş yapan kullanıcının ID'sini Claims üzerinden alıyoruz
            var userId = User.FindFirstValue("UserId");
            listing.UserId = int.Parse(userId);
            listing.CreatedAt = DateTime.Now;
            listing.IsActive = true;

            _context.Listings.Add(listing);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // İLANA YANIT VER / BAŞVURU YAP (POST)
        [HttpPost]
        public async Task<IActionResult> SubmitApplication(int listingId, string message, string contactInfo)
        {
            // Giriş yapan kullanıcının ID'sini alıyoruz
            var userIdString = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            // Yeni bir başvuru/yanıt nesnesi oluşturuyoruz
            var application = new Application
            {
                ListingId = listingId,
                UserId = int.Parse(userIdString), // Yanıtı yazan kişi
                Message = message,
                ContactInfo = contactInfo,
                CreatedAt = DateTime.Now
            };

            // Veritabanına kaydediyoruz
            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            // İşlem başarılı mesajı gönderip tekrar ilan detayına yönlendiriyoruz
            TempData["Success"] = "Yanıtınız başarıyla iletildi!";
            return RedirectToAction("Details", new { id = listingId });
        }



    }
}