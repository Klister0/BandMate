using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace BandMate.Models
{
    public class Listing
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Instrument { get; set; }

        public decimal? Price { get; set; }
        public string City { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public User User { get; set; }
        public Category Category { get; set; }
        public ICollection<Application> Applications { get; set; }
    }
}