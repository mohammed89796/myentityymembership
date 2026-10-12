


namespace MyEntityMembershipService.DTOs.Entity.VenueDtos
{
    public class GetVenueMobileDetailsDto
    {
        // public GetVenueHeaderDetails VenueHeader { get; set; }
        public int BranchID { get; set; }
        public int FacilityID { get; set; }
        public string FacilityName { get; set; }
        public string VenueImageURL { get; set; }
        public string VenueName { get; set; }
        public double? Rating { get; set; }
        public GetVenueAboutDetailsDto VenueAbout { get; set; }
        public GetVenueFeaturesDto VenueFeatures { get; set; }
        public List<GetVenueMediaDto> VenueMedia { get; set; }
        public VenueOperatingHours OperatingHours { get; set; }
    }
    public class GetVenueHeaderDetails
    {
        public string VenueImageURL { get; set; }
        public string VenueName { get; set; }
    }
    public class GetVenueAboutDetailsDto
    {
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string Governnorate { get; set; }
    }
    public class GetVenueFeaturesDto
    {
        public int VenueCapacity { get; set; }
        public string Dimensions { get; set; }
        public string? Type { get; set; }
        public string TurfType { get; set; }
        public List<string> Utilities { get; set; }
        public List<string> Accessibility { get; set; }
        public List<string> Amenities { get; set; }
    }
    public class GetVenueMediaDto
    {
        public int ID { get; set; }
        public string ImageUrl { get; set; }

    }

    public class VenueOperatingHoursMobileDto
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
