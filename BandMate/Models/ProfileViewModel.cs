using System.Collections.Generic;

namespace BandMate.Models
{
    public class ProfileViewModel
    {
        public List<Listing> MyListings { get; set; } = new List<Listing>();
        public List<Application> ReceivedApplications { get; set; } = new List<Application>();
    }
}