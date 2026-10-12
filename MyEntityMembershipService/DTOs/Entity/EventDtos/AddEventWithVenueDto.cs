using DocumentFormat.OpenXml.Features;

namespace MyEntityMembershipService.DTOs.Entity.EventDtos
{
    public class AddEventWithVenueDto : AddEventDto
    {


        public int VenueId { get; set; }

        public string? VenueName { get; set; }


        public string? VenueImageUrl { get; set; }
        public string? VenueLocation { get; set; }

        public string? FacilityName { get; set; }
        public int? FacilityId { get; set; }

        public string? FacilityImageUrl { get; set; }
        public string? FacilityLocation { get; set; }

        public string? BranchName { get; set; }
        public string? BranchImageUrl { get; set; }
        public int? BranchId { get; set; }
        public string? BranchLocation { get; set; }

    }


    public class AddEventDto
    {
        public string EventName { get; set; }
        public DateOnly EventDate { get; set; }
        public string? Description { get; set; }
        public TimeOnly From { get; set; }
        public TimeOnly To { get; set; }
        public string? EventLocation { get; set; }
        public EventType EventType { get; set; }
        public IFormFile? EventPicture { get; set; }
        public int ProgramId { get; set; } // ID of the program this event belongs to
        public List<string> EventAdmins { get; set; } = new List<string>(); // Admins associated with the event
        public string? CreatedBy { get; set; }

        public string? EventAdminsJson { get; set; }

        public bool? EventAssessment { get; set; }
        public DateOnly? EventEndDate { get; set; }
    }
}
