

namespace MyEntityMembershipService.DTOs.Entity.FacilityDtos
{
    public class EditFacilityDetailsDto
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public string FacilityName { get; set; }
        public string? FacilityImageUrl { get; set; }
        public int FacilityTypeId { get; set; }
        public string? Description { get; set; }
        public string CountryCodeID { get; set; }
        public int PhoneNumber { get; set; }
        public int FacilityPremisesTypeID { get; set; }
        public EditFacilityLocationDto FacilityLocation { get; set; }
        public List<EditFacilityAmenityDto> FacilityAmenities { get; set; }
        public List<EditFacilityServiceDto> FacilityServices { get; set; }
        public List<EditFacilityAccessibilityOptionDto> FacilityAccessibilityOptions { get; set; }
        public List<EditFacilitySportDto> SportsList { get; set; }
        public List<EditFacilityImageDto> FacilityUploads { get; set; }
        public NewFacilityOperatingHours FacilityOperatingHours { get; set; }
    }

    // Supporting DTOs
    public class EditFacilityLocationDto
    {
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }

    public class EditFacilityAmenityDto
    {
        public int AmenityId { get; set; }
        public string AmenityName { get; set; }
    }

    public class EditFacilityServiceDto
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
    }

    public class EditFacilityAccessibilityOptionDto
    {
        public int AccessibilityOptionId { get; set; }
        public string AccessibilityOptionName { get; set; }
    }

    public class EditFacilitySportDto
    {
        public int SportId { get; set; }
        public string SportName { get; set; }
    }

    public class EditFacilityImageDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
    }

}
