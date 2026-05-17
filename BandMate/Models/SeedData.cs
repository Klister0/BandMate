using Microsoft.EntityFrameworkCore;

namespace BandMate.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // 1. Önce Rolleri Kontrol Et ve Ekle
                if (!context.Roles.Any())
                {
                    context.Roles.AddRange(
                        new Role { Name = "Admin" },
                        new Role { Name = "User" }
                    );
                }

                // 2. Kategorileri Kontrol Et ve Ekle
                if (!context.Categories.Any())
                {
                    context.Categories.AddRange(
                        new Category { Name = "Grup Arayanlar" },
                        new Category { Name = "Grubuna Eleman Arayanlar" },
                        new Category { Name = "Ekipman Alışverişi" },
                        new Category { Name = "Özel Ders Verenler/Alanlar" }
                    );
                }

                context.SaveChanges();
            }
        }
    }
}