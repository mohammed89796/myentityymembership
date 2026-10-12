using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class SportsAccommodated : BaseEntity
    {
        public string name { get; set; }
        public ICollection<FacilitySportsAccommodated>? FacilitySportsAccommodated { get; set; }

    }
}
