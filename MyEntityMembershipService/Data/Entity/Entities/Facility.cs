using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class Facility : BaseEntity
    {
        public string name { get; set; }
        public string? Image { get; set; }
        public string? email { get; set; }
        public string? phoneNumber { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string address { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
       // public int? OrganizationId { get; set; }
        
       // public OrganizationEntity? Organization { get; set; }

        public int BranchId { get; set; }
        public virtual Branch Branch { get; set; }
        public int? FacilityTypeId { get; set; }
        public FacilityType? FacilityType { get; set; }
        public ICollection<Venue>? Venues { get; set; }
        public ICollection<FacilityAccessibilityFeatures>? FacilityAccessibilityFeatures { get; set; }
        public ICollection<FacilityAmenities>? FacilityAmenities { get; set; }
        public ICollection<FacilitySportsAccommodated>? FacilitySportsAccommodated { get; set; }
        //public ICollection<AccessedBranchesFacilities>? AccessedBranchesFacilities { get; set; }
        //public ICollection<AccessedFacilitiesVenues>? AccessedFacilitiesVenues { get; set; } = new HashSet<AccessedFacilitiesVenues>();


        public FacilityOperatingHours? FacilityOperatingHours { get; set; }
    }
}
