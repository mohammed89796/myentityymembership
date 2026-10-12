using System.ComponentModel.DataAnnotations.Schema;
using MyEntityMembershipService.Data.Entity.Entities.Enums;
using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class NewFacility : BaseEntity
    {
        public int BranchId { get; set; }
        public string FacilityName { get; set; }
        public string? FacilityImageUrl { get; set; }
        public int FacilityTypeId { get; set; }
        
        [ForeignKey("FacilityTypeId")]
        public FacilityType? FacilityType { get; set; }
        public string? Description { get; set; }
        public string CountryCodeID { get; set; }
        public int PhoneNumber { get; set; }
        public int FacilityPremisesTypeID { get; set; }
        public virtual FacilityLocation FacilityLocation { get; set; }
        public NewFacilityOperatingHours? FacilityOperatingHours { get; set; }
        public ICollection<NewFacilityAmenities>? FacilityAmenities { get; set; }
        public ICollection<FacilityServices>? FacilityServices { get; set; }
        public ICollection<NewFacilityAccessibilityOptions>? FacilityAccessibilityOptions { get; set; }
        public ICollection<NewFacilitySports>? SportsList { get; set; }

        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }
        public ICollection<FacilityImages>? FacilityUploads { get; set; }
        public ICollection<NewVenue>? NewVenues { get; set; } = new HashSet<NewVenue>();



        public ICollection<AccessedBranchesFacilities>? AccessedBranchesFacilities { get; set; }= new HashSet<AccessedBranchesFacilities>();
        public ICollection<AccessedFacilitiesVenues>? AccessedFacilitiesVenues { get; set; } = new HashSet<AccessedFacilitiesVenues>();


    }

}
