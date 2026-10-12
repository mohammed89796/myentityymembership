using System.ComponentModel.DataAnnotations.Schema;
using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{

    public class VenueBooking : BaseEntity
    {
        public int VenueId { get; set; }

        [ForeignKey("VenueId")]
        public virtual NewVenue Venue { get; set; }
        public DateTime BookingDate { get; set; }
        public int BookingType { get; set; }
        public virtual ICollection<VenueBookingSlot> BookingSlots { get; set; }
        public string? EventName { get; set; }
        public int? EventypeID { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public bool? IsNonMember { get; set; }
        public string? MemberID { get; set; }
        public string? MemberName { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
