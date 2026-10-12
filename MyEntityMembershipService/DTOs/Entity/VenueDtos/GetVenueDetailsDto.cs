


namespace MyEntityMembershipService.DTOs.Entity.VenueDtos
{
    public class GetVenueDetailsDto
    {
        public int BranchID { get; set;}
        public string FacilityName { get; set; }
        public VenueHeaderDetailsDto HeaderDetails { get; set; }
        public VenueFeaturesDetailsDto FeaturesDetails { get; set; }
        public VenueOperatingHoursDetailsDto OperatingHours { get; set; }
        public VenueLocationDetailsDto LocationDetails { get; set; }
        public List<VenueImages> VenueImages { get; set; }

    }
    public class VenueLocationDetailsDto
    {
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
    public class VenueHeaderDetailsDto
    {
        public string VenueName { get; set; }
        public string VanueImageURL { get; set; }
        public string FacilityName { get; set; }
        public string CountryCode { get; set; }
        public string PhoneNumber { get; set; }
        public string BranchName { get; set; }
        public string BranchImageURL { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
    }
    public class VenueFeaturesDetailsDto
    {
        public int VenueCapacity { get; set; }
        public string Dimensions { get; set; }
        public string Type { get; set; }
        public string TrufType { get; set; }
        public List<string> Utilities { get; set; }
        public List<string> Accessibility { get; set; }
        public List<string> Amenities { get; set; }
    }

    public class VenueOperatingHoursDetailsDto
    {
        public TimeOnly? SundayOpenAt { get; set; }
        public TimeOnly? SundayCloseAt { get; set; }
        public bool? SundayIsWorking { get; set; }
        public TimeOnly? MondayOpenAt { get; set; }
        public TimeOnly? MondayCloseAt { get; set; }
        public bool? MondayIsWorking { get; set; }
        public TimeOnly? TuesdayOpenAt { get; set; }
        public TimeOnly? TuesdayCloseAt { get; set; }
        public bool? TuesdayIsWorking { get; set; }
        public TimeOnly? WednesdayOpenAt { get; set; }
        public TimeOnly? WednesdayCloseAt { get; set; }
        public bool? WednesdayIsWorking { get; set; }
        public TimeOnly? ThursdayOpenAt { get; set; }
        public TimeOnly? ThursdayCloseAt { get; set; }
        public bool? ThursdayIsWorking { get; set; }
        public TimeOnly? FridayOpenAt { get; set; }
        public TimeOnly? FridayCloseAt { get; set; }
        public bool? FridayIsWorking { get; set; }
        public TimeOnly? SaturdayOpenAt { get; set; }
        public TimeOnly? SaturdayCloseAt { get; set; }
        public bool? SaturdayIsWorking { get; set; }
    }
}
