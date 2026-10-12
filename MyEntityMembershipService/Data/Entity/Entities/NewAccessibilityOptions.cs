using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class NewAccessibilityOptions : BaseEntity
    {
        public string OptionName { get; set; }
        public ICollection<NewFacilityAccessibilityOptions>? FacilityAccessibilityOptions { get; set; }
        public ICollection<NewVenueAccessibilityOptions>? VenueAccessibilityOptions { get; set; }

    }
}
