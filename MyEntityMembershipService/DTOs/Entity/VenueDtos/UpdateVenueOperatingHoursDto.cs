namespace MyEntityMembershipService.DTOs.Entity.VenueDtos
{
    public class UpdateVenueOperatingHoursDto
    {
        public int Id { get; set; }
        public TimeOnly? sundayOpenAt { get; set; }
        public TimeOnly? sundayCloseAt { get; set; }
        public bool? sundayIsWorking { get; set; }
        public TimeOnly? mondayOpenAt { get; set; }
        public TimeOnly? mondayCloseAt { get; set; }
        public bool? mondayIsWorking { get; set; }
        public TimeOnly? tuesdayOpenAt { get; set; }
        public TimeOnly? tuesdayCloseAt { get; set; }
        public bool? tuesdayIsWorking { get; set; }
        public TimeOnly? wednesdayOpenAt { get; set; }
        public TimeOnly? wednesdayCloseAt { get; set; }
        public bool? wednesdayIsWorking { get; set; }
        public TimeOnly? thursdayOpenAt { get; set; }
        public TimeOnly? thursdayCloseAt { get; set; }
        public bool? thursdayIsWorking { get; set; }
        public TimeOnly? fridayOpenAt { get; set; }
        public TimeOnly? fridayCloseAt { get; set; }
        public bool? fridayIsWorking { get; set; }
        public TimeOnly? saturdayOpenAt { get; set; }
        public TimeOnly? saturdayCloseAt { get; set; }
        public bool? saturdayIsWorking { get; set; }
        public int VenueId { get; set; }
    }
}
