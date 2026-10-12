

namespace MyEntityMembershipService.DTOs.Entity.VenueDtos
{
    public class AddVenueConfigurationDto
    {
        public int SlotTime { get; set; }
        public int PricePerSlot { get; set; }
        public int MinSlotsBooked { get; set; }
        public int MaxSlotsBooked { get; set; }
        public int CancellationTime { get; set; }
        public int BookingAvailability { get; set; }
        public string BookingAvailabilityUnit { get; set; }
        public bool IsVisible { get; set; }
        public bool AllowBooking { get; set; }
        public int VenueID { get; set; }
        public List<PricingRuleDto>? PricingRules { get; set; }
    }

    public class PricingRuleDto
    {
        public string Day { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public int CustomPrice { get; set; }

    }
}
