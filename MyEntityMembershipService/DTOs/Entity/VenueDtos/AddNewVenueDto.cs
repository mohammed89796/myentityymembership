

namespace MyEntityMembershipService.DTOs.Entity.VenueDtos
{
    public class AddNewVenueDto
    {
        public int? CreatedVenueID { get; set; }
        public int FacilityID { get; set; }
        public string VenueName { get; set; }
        public int VenueType { get; set; }
        public int VenueCapacity { get; set; }
        public bool? AllowMultipleEventsAtSameTime { get; set; }
        public IFormFile VenueImg { get; set; }
        public List<IFormFile> VenueUploads { get; set; }
        public List<int> VenueUtilities { get; set; }
        public List<int> VenueAccessibilityOptions { get; set; }
        public List<int> VenueAmenities { get; set; }
        public string? FieldDimensions { get; set; }
        public List<int>? TurfTypes { get; set; }
        public string? Description { get; set; }
        public VenueLocationDto VenueLocation { get; set; }

    }

    public class VenueLocationDto
    {
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }

    public class VenueOperatingHoursDto
    {
        public int VenueID { get; set; }
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
