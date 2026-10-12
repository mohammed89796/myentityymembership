

using System.ComponentModel.DataAnnotations.Schema;

namespace MyEntityMembershipService.DTOs.Entity.VenueDtos
{
    public class BaseBookingDto
    {
        public int VenueId { get; set; }
        public DateTime BookingDate { get; set; }
        public List<TimeSlotBookingDto> TimeSlots { get; set; }
        public int BookingTypeID { get; set; }
    }

    public class EventBookingDto : BaseBookingDto
    {
        public string EventName { get; set; }
        public decimal Price { get; set; }
        public int EventypeID { get; set; }
        public string Description { get; set; }
        public bool IsNonMember { get; set; }
    }
    public class EventMemberBookingDto : BaseBookingDto
    {
        public string? MemberID { get; set; }
        public decimal Price { get; set; }
        public string MemberName { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class TimeSlotBookingDto
    {
        public string SlotStart { get; set; }
        public string SlotEnd { get; set; }
    }
}
