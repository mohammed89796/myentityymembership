using System.ComponentModel.DataAnnotations.Schema;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class VenueImages : BaseEntity
    {
        public string ImageUrl { get; set; }
        public int VenueID { get; set; }

        [ForeignKey("VenueID")]
        public virtual NewVenue Venue { get; set; }
    }
}
