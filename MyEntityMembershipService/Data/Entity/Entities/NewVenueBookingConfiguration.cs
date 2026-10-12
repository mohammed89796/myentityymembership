using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class NewVenueBookingConfiguration : BaseEntity
    {
        public int SlotTime { get; set; }
        public int PricePerSlot { get; set; }
        public int MinSlotsBooked { get; set; }
        public int MaxSlotsBooked { get; set; }
        public int CancellationTime { get; set; }
        public int BookingAvailability { get; set; }
        public string BookingAvailabilityUnit { get; set; } = "Days";
        public bool IsVisible { get; set; }
        public bool AllowBooking { get; set; }
        public int VenueID { get; set; }

        [ForeignKey("VenueID")]
        public NewVenue? Venue { get; set; }

        [JsonIgnore]
        public ICollection<NewVenuePricingRules>? PricingRules { get; set; }

    }
}
