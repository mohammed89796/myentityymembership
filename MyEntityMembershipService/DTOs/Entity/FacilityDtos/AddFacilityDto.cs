namespace MyEntityMembershipService.DTOs.Entity.FacilityDtos
{
    public class AddFacilityDto
    {
        public int? CreatedFacilityID { get; set; }
        public int BranchId { get; set; }
        public string FacilityName { get; set; }
        public IFormFile FacilityImage { get; set; }
        public int FacilityTypeId { get; set; }
        public string? Description { get; set; }
        public int FacilityPremisesTypeID { get; set; }
        public List<int>? FacilityAmenities { get; set; }
        public List<int>? FacilityServices { get; set; }
        public List<int>? FacilityAccessibilityOptions { get; set; }
        public List<int>? SportsList { get; set; }
        public virtual FacilityLocationDto FacilityLocation { get; set; }
        public List<IFormFile>? FacilityUploads { get; set; }
        public string CountryCodeID { get; set; }
        public int PhoneNumber { get; set; }
    }


    public class FacilityLocationDto
    {
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
    public class FacilityOperatingHoursDto
    {
        public int FacilityId { get; set; }
        public string? SundayOpenAt { get; set; }
        public string? SundayCloseAt { get; set; }
        public bool? SundayIsWorking { get; set; }
        public string? MondayOpenAt { get; set; }
        public string? MondayCloseAt { get; set; }
        public bool? MondayIsWorking { get; set; }
        public string? TuesdayOpenAt { get; set; }
        public string? TuesdayCloseAt { get; set; }
        public bool? TuesdayIsWorking { get; set; }
        public string? WednesdayOpenAt { get; set; }
        public string? WednesdayCloseAt { get; set; }
        public bool? WednesdayIsWorking { get; set; }
        public string? ThursdayOpenAt { get; set; }
        public string? ThursdayCloseAt { get; set; }
        public bool? ThursdayIsWorking { get; set; }
        public string? FridayOpenAt { get; set; }
        public string? FridayCloseAt { get; set; }
        public bool? FridayIsWorking { get; set; }
        public string? SaturdayOpenAt { get; set; }
        public string? SaturdayCloseAt { get; set; }
        public bool? SaturdayIsWorking { get; set; }
    }




}
