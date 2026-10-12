using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class NewCoreUtilities : BaseEntity
    {
        public string UtilityName { get; set; }
        public ICollection<NewVenueUtilities>? VenueUtilities { get; set; }

    }
}
