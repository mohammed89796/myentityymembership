using System.ComponentModel.DataAnnotations.Schema;
using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class NewVenue : BaseEntity
    {
        public string Name { get; set; }
        public virtual NewVenueTypes VenueType { get; set; }
        public int VenueCapacity { get; set; }
        public bool? AllowMultipleEventsAtSameTime { get; set; }
        public virtual VenueLocation VenueLocation { get; set; }
        public virtual VenueOperatingHours? OperatingHours { get; set; }
        public ICollection<NewVenueUtilities>? VenueUtilities { get; set; }
        public ICollection<NewVenueAccessibilityOptions>? VenueAccessibilityOptions { get; set; }
        public ICollection<NewVenueAmenities> VenueAmenities { get; set; }
        public string? FieldDimensions { get; set; }
        public ICollection<NewVenueTurfTypes>? TurfTypes { get; set; }
        public ICollection<VenueImages>? VenueUploads { get; set; }
        public string VenueImageURL { get; set; }
        public string? Description { get; set; }
        public int FacilityId { get; set; }

        [ForeignKey("FacilityId")]
        public NewFacility? Facility { get; set; }
        public ICollection<VenueReviews> VenueReviews { get; set; }


        public ICollection<AccessedFacilitiesVenues>? AccessedFacilitiesVenues { get; set; } = new HashSet<AccessedFacilitiesVenues>();


    }
}
