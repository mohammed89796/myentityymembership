

namespace MyEntityMembershipService.DTOs.Entity.VenueDtos
{
    public class GetVenueBookingsMobileDto
    {
        public int VenueID { get; set; }
        public string VenueImage { get; set; }
        public string VenueName { get; set; }
        public int VenueRating { get; set; }
        public int VenueRecommendation { get; set; }
        public List<GetVenueBookedSlotsMobileDto> BookedSlots { get; set; }
    }
    public class GetVenueBookedSlotsMobileDto
    {
        public int BookingID { get; set; }
        public DateTime BookingDate { get; set; }
        public string VenueName { get; set; }
        public TimeOnly SlotStartTime { get; set; }
        public TimeOnly SlotEndTime { get; set; }
        public string Address { get; set; }
        public string FacilityName { get; set; }
    }


}
