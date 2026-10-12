using System.ComponentModel.DataAnnotations.Schema;
using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class VenueReviews : BaseEntity
    {
        public string ReviewerID { get; set; }
        public string Review { get; set; }
        public int Rating { get; set; }
        public bool Recommend { get; set; }
        public int VenueID { get; set; }
        
        [ForeignKey("VenueID")]
        public NewVenue Venue { get; set; }
    }
}
