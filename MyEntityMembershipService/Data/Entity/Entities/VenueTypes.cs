using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class VenueTypes : BaseEntity
    {
        public string TypeName { get; set; }
        public ICollection<NewVenueTypes>? NewVenueTypes { get; set; }

    }
}
