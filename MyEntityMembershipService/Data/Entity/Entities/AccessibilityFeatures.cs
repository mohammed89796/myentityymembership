using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class AccessibilityFeatures : BaseEntity
    {
        public string name { get; set; }
        public ICollection<VenueAccessibilityFeatures>? VenueAccessibilityFeatures { get; set; }
        public ICollection<FacilityAccessibilityFeatures>? FacilityAccessibilityFeatures { get; set; }
    }
}
