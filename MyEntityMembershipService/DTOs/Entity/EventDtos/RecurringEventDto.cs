
using MyEntityMembershipService.Data.Entity.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Entity.EventDtos
{
    public class RecurringEventDto
    {
        public string EventName { get; set; }
        public string? Description { get; set; }
        public string? EventPictureUrl { get; set; }
        public EventType EventType { get; set; }
        public DateOnly? EventDate { get; set; }
        public TimeOnly? From { get; set; }
        public TimeOnly? To { get; set; }
        public int ProgramId { get; set; }
        public string EventAdminsJson { get; set; }
        public List<string>? EventAdmins { get; set; } =new List<string>();
        public int VenueId { get; set; }
        public string? VenueName { get; set; }

        public string? VenueImageUrl { get; set; }

        public string? FacilityName { get; set; }

        public string? BranchName { get; set; }
    }
}
