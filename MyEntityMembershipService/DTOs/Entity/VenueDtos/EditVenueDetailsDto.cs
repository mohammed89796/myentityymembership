





namespace MyEntityMembershipService.DTOs.Entity.VenueDtos
{
    public class EditVenueDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int VenueTypeId { get; set; }
        public string VenueTypeName { get; set; }
        public int VenueCapacity { get; set; }
        public string? Description { get; set; }
        public string? FieldDimensions { get; set; }
        public int FacilityId { get; set; }
        public string VenueImageURL { get; set; }
        public EditVenueLocationDto VenueLocation { get; set; }
        public List<EditVenueUtilityDto> VenueUtilities { get; set; }
        public List<EditVenueAccessibilityOptionDto> VenueAccessibilityOptions { get; set; }
        public List<EditVenueAmenityDto> VenueAmenities { get; set; }
        public List<EditVenueTurfTypeDto> TurfTypes { get; set; }
        public List<EditVenueImageDto> VenueUploads { get; set; }
        public EditVenueOperatingHoursDetailsDto OperatingHours { get; set; }
    }

    // Supporting DTOs
    public class EditVenueLocationDto
    {
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }

    public class EditVenueUtilityDto
    {
        public int UtilityId { get; set; }
        public string UtilityName { get; set; }
    }

    public class EditVenueAccessibilityOptionDto
    {
        public int AccessibilityOptionId { get; set; }
        public string AccessibilityOptionName { get; set; }
    }

    public class EditVenueAmenityDto
    {
        public int AmenityId { get; set; }
        public string AmenityName { get; set; }
    }

    public class EditVenueTurfTypeDto
    {
        public int TurfTypeId { get; set; }
        public string TurfTypeName { get; set; }
    }

    public class EditVenueImageDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
    }

    public class EditVenueOperatingHoursDetailsDto
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
