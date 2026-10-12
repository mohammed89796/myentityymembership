using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class Amenities : BaseEntity
    {
        public string name { get; set; }
        public ICollection<NewFacilityAmenities>? FacilityAmenities { get; set; }
        public ICollection<NewVenueAmenities>? VenueAmenities { get; set; }
    }
}
