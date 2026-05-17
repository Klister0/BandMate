using System.ComponentModel.DataAnnotations;

namespace BandMate.Models
{
    public class Application
    {
        [Key]
        public int Id { get; set; }
        public int ListingId { get; set; }
        public int UserId { get; set; }

        [Required]
        public string Message { get; set; }
        [Required]
        public string ContactInfo { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Listing Listing { get; set; }
        public User User { get; set; }
    }
}