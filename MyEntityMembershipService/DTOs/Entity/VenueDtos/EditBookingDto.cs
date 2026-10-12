namespace MyEntityMembershipService.DTOs.Entity.VenueDtos
{
    public class BaseEditBookingDto
    {
        public int Id { get; set; }
        public int VenueId { get; set; }
        public DateTime BookingDate { get; set; }
        public List<EditTimeSlotBookingDto> TimeSlots { get; set; }
        public int BookingTypeID { get; set; }
    }

    public class EditEventBookingDto : BaseEditBookingDto
    {
        public string EventName { get; set; }
        public decimal Price { get; set; }
        public int EventypeID { get; set; }
        public string Description { get; set; }
        public bool IsNonMember { get; set; }
    }
    public class EditEventMemberBookingDto : BaseEditBookingDto
    {
        public string? MemberID { get; set; }
        public decimal Price { get; set; }
        public string MemberName { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class EditTimeSlotBookingDto
    {
        public TimeOnly SlotStart { get; set; }
        public TimeOnly SlotEnd { get; set; }
    }
    public class GeneralEditTimeSlotDto
    {
        public string SlotStart { get; set; }
        public string SlotEnd { get; set; }
    }
    public class GeneralEditDto
    {
        public int BookingID { get; set; }
        public DateTime? BookingDate { get; set; }
        public List<GeneralEditTimeSlotDto> TimeSlots { get; set; }

    }
}
