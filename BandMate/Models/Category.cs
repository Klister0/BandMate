using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace BandMate.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Listing> Listings { get; set; }
    }
}