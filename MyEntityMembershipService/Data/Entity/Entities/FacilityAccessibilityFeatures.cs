namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class FacilityAccessibilityFeatures
    {
        public int FacilityId { get; set; }
        public Facility? Facility { get; set; }

        public int AccessibilityFeaturesId { get; set; }
        public AccessibilityFeatures? AccessibilityFeatures { get; set; }
    }
}
