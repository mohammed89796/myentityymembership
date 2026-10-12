using System.ComponentModel.DataAnnotations.Schema;
using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class NewVenueAmenities : BaseEntity
    {
        public int VenueID { get; set; }
        public NewVenue? Venue { get; set; }
        public int AmenityID { get; set; }
        
        [ForeignKey("AmenityID")]
        public Amenities? Amenity { get; set; }
    }
}
