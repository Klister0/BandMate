using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BandMate.Models;

namespace BandMate.Controllers
{
    [Authorize(Roles = "Admin")] // Sadece veritabanında RoleId'si Admin (1) olanlar girebilir
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Admin Paneli Ana Sayfası (Dashboard)
        // URL: /Admin veya /Admin/Index
        public async Task<IActionResult> Index()
        {
            ViewBag.UserCount = await _context.Users.CountAsync();
            ViewBag.ListingCount = await _context.Listings.CountAsync();
            ViewBag.CategoryCount = await _context.Categories.CountAsync();

            return View();
        }

        // Tüm İlanları Listeleme ve Moderasyon Sayfası
        // URL: /Admin/ManageListings
        public async Task<IActionResult> ManageListings()
        {
            var listings = await _context.Listings
                .Include(l => l.Category)
                .Include(l => l.User)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

            return View(listings);
        }

        // İlanı Veritabanından Tamamen Silme Aksiyonu
        [HttpPost]
        public async Task<IActionResult> DeleteListing(int id)
        {
            var listing = await _context.Listings.FindAsync(id);
            if (listing != null)
            {
                _context.Listings.Remove(listing);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ManageListings");
        }
    }
}