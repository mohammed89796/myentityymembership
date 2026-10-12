using MyEntityMembershipService.Data;


namespace MyEntityMembershipService.DTOs.Entity.VenueDtos
{
    public class UpdateVenueDto : BaseEntity
    {
        public string name { get; set; }
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
        public List<int>? VenueStadFacilitiesId { get; set; }
        public List<int>? accessibilityFeaturesId { get; set; }
    }
}
