namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class VenueAccessibilityFeatures
    {
        public int VenueId { get; set; }
        public Venue? Venue { get; set; }

        public int AccessibilityFeaturesId { get; set; }
        public AccessibilityFeatures? AccessibilityFeatures { get; set; }
    }
}
