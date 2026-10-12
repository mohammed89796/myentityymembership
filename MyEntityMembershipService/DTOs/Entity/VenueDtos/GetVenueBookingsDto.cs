

namespace MyEntityMembershipService.DTOs.Entity.VenueDtos
{
    public class GetVenueBookingsDto
    {
        public DateTime StartDate { get; set; }
        public string View { get; set; } //Either day, week or month
        public int PageNumber { get; set; }
        public int FacilityId { get; set; }
    }

}
