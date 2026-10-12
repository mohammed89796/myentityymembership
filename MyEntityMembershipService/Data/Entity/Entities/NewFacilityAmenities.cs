using System.ComponentModel.DataAnnotations.Schema;
using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class NewFacilityAmenities : BaseEntity
    {
        public int FacilityId { get; set; }
        public NewFacility? Facility { get; set; }

        public int AmenityID { get; set; }
        [ForeignKey("AmenityID")]
        public Amenities? Amenity { get; set; }
    }
}
