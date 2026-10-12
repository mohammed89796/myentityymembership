




namespace MyEntityMembershipService.DTOs.Entity.VenueDtos
{
    public class EditVenueDto
    {
        public int VenueId { get; set; }
        public string Name { get; set; }
        public int VenueCapacity { get; set; }
        public int VenueTypeId { get; set; }
        public VenueLocationDto? VenueLocation { get; set; }
        public bool? AllowMultipleEventsAtSameTime { get; set; }=true;
        // public EditVenueOperatingHoursDto? VenueOperatingHours { get; set; }
        public List<int>? VenueUtilities { get; set; }
        public List<int>? VenueAccessibilityOptions { get; set; }
        public List<int>? VenueAmenities { get; set; }
        public List<int>? TurfTypes { get; set; }
        public string? FieldDimensions { get; set; }
        public string? Description { get; set; }
        public List<IFormFile>? VenueUploads { get; set; }
        public IFormFile? VenueImageURL { get; set; }
    }


    public class EditVenueOperatingHoursDto
    {
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
        public int VenueID { get; set; }
    }

}
