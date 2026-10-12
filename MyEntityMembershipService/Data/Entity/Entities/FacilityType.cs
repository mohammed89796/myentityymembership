using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class FacilityType : BaseEntity
    {
        public string name { get; set; }
        public ICollection<Facility>? Facilities { get; set; }
        public ICollection<NewFacility>? NewFacilities { get; set; }
    }
}
