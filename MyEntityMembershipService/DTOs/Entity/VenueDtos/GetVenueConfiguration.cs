using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;



namespace MyEntityMembershipService.DTOs.Entity.VenueDtos
{
    public class GetVenueConfiguration
    {
        public int? ConfigID { get; set; }
        public int SlotTime { get; set; }
        public int PricePerSlot { get; set; }
        public int MinSlotsBooked { get; set; }
        public int MaxSlotsBooked { get; set; }
        public int CancellationTime { get; set; }
        public int BookingAvailability { get; set; }
        public string BookingAvailabilityUnit { get; set; }
        public bool IsVisible { get; set; }
        public bool AllowBooking { get; set; }
        public List<PricingRulesDto>? PricingRules { get; set; }

    }
    public class PricingRulesDto
    {
        public DayOfWeek? Day { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public int CustomPrice { get; set; }


    }
}
