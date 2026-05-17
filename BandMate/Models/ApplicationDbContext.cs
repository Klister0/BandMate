using Microsoft.EntityFrameworkCore;

namespace BandMate.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Listing> Listings { get; set; }
        public DbSet<Application> Applications { get; set; }

        // HATAYI ÇÖZEN KISIM BURASI:
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Uygulama (Application) tablosundaki UserId silindiğinde zincirleme silmeyi kapatıyoruz.
            modelBuilder.Entity<Application>()
                .HasOne(a => a.User)
                .WithMany(u => u.Applications)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Cascade yerine Restrict yaparak yolu tekilleştirdik.
        }
    }
}