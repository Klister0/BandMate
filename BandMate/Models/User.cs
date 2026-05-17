using System.ComponentModel.DataAnnotations;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

namespace BandMate.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public int RoleId { get; set; }

        [Required]
        public string FullName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        public string? City { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Bio { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Role Role { get; set; }
        public ICollection<Listing> Listings { get; set; }
        public ICollection<Application> Applications { get; set; }
    }
}