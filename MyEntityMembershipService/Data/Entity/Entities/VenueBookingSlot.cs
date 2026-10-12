using System.ComponentModel.DataAnnotations.Schema;
using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class VenueBookingSlot : BaseEntity
    {
        public int VenueBookingId { get; set; }

        [ForeignKey("VenueBookingId")]
        public virtual VenueBooking VenueBooking { get; set; }
        public TimeOnly SlotStart { get; set; }
        public TimeOnly SlotEnd { get; set; }
    }
}
