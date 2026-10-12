using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class VenueBookingTypes : BaseEntity
    {
        public string IconURL { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

    }
}
