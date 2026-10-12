

namespace MyEntityMembershipService.DTOs.Entity.FacilityDtos
{
    public class FacilityDetailsDto
    {
        public int FacilityID { get; set; }
        public FacilityHeaderDetailsDto HeaderDetails { get; set; }
        public FacilityFeaturesDto FeaturesDetails { get; set; }
        public OperatingHoursDto OperatingHours { get; set; }
        public List<FacilityImages> FacilityImages { get; set; }
        public FacilityLocationDetailsDto LocationDetails { get; set; }
    }
    public class FacilityHeaderDetailsDto
    {
        public string FacilityImageURL { get; set; }
        public string FacilityName { get; set; }
        public string BranchName { get; set; }
        public string BranchImageURL { get; set; }
        public string Description { get; set; }
        public string FacilityTypeName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public int NumberOfVenues { get; set; }
    }
    public class FacilityFeaturesDto
    {
        public List<string> FacilitySports { get; set; }
        public List<string> FacilityAccessibility { get; set; }
        public List<string> FacilityAmenities { get; set; }
    }
    public class FacilityLocationDetailsDto
    {
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }

    public class OperatingHoursDto
    {
        public TimeOnly? SundayOpenAt { get; set; }
        public TimeOnly? SundayCloseAt { get; set; }
        public bool SundayIsWorking { get; set; }
        public TimeOnly? MondayOpenAt { get; set; }
        public TimeOnly? MondayCloseAt { get; set; }
        public bool MondayIsWorking { get; set; }
        public TimeOnly? TuesdayOpenAt { get; set; }
        public TimeOnly? TuesdayCloseAt { get; set; }
        public bool TuesdayIsWorking { get; set; }
        public TimeOnly? WednesdayOpenAt { get; set; }
        public TimeOnly? WednesdayCloseAt { get; set; }
        public bool WednesdayIsWorking { get; set; }
        public TimeOnly? ThursdayOpenAt { get; set; }
        public TimeOnly? ThursdayCloseAt { get; set; }
        public bool ThursdayIsWorking { get; set; }
        public TimeOnly? FridayOpenAt { get; set; }
        public TimeOnly? FridayCloseAt { get; set; }
        public bool FridayIsWorking { get; set; }
        public TimeOnly? SaturdayOpenAt { get; set; }
        public TimeOnly? SaturdayCloseAt { get; set; }
        public bool SaturdayIsWorking { get; set; }
    }


}
