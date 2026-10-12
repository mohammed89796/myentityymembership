using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class Venue : BaseEntity
    {
        public string name { get; set; }
        public string? Image { get; set; }

        public string? sportAccommodated { get; set; }
        public string? typeOfTurf { get; set; }
        public string? typeOfCourtSurface { get; set; }
        public string? venueLocation { get; set; }
        public string? playerFacilities { get; set; }
        public string? matchFormats { get; set; }
        public string? fieldDimensions { get; set; }
        public string? goalSize { get; set; }
        public string? basketHeight { get; set; }
        public string? courtDimensions { get; set; }
        public string? seatingAvailability { get; set; }
        public string? playerAndOfficialFacilities { get; set; }
        public ICollection<VenueStadFacility>? VenueStadFacilities { get; set; }
        public ICollection<VenueAccessibilityFeatures>? VenueAccessibilityFeatures { get; set; }
        public int FacilityId { get; set; }
        public Facility? Facility { get; set; }
        public VenueOperatingHours? VenueOperatingHours { get; set; }
        //public ICollection<AccessedFacilitiesVenues>? AccessedFacilitiesVenues { get; set; } = new HashSet<AccessedFacilitiesVenues>();

    }
}
