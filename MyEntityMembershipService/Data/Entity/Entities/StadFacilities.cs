using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class StadFacilities : BaseEntity
    {
        public string name { get; set; }
        public ICollection<VenueStadFacility>? VenueStadFacilities { get; set; }
        public ICollection<FacilityAmenities>? FacilityAmenities { get; set; }
    }
}
