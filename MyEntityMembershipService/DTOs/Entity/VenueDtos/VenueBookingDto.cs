
using MyEntityMembershipService.Configurations;

namespace MyEntityMembershipService.DTOs.Entity.VenueDtos
{
    public class VenueBookingDto
    {
        public int? Id { get; set; }

        public int TotalNumberOfVenues { get; set; }
        public int VenueId { get; set; }
        public string VenueName { get; set; }
        public NewVenueBookingConfigurationDto VenueConfiguration { get; set; }
        public VenueOperatingHoursDto VenueOperatingHours { get; set; }
        public List<CalendarEventDto> Bookings { get; set; }
    }

    public class CalendarEventDto
    {
        public int? Id { get; set; }

        public string Title { get; set; }
        public DateTime BookingDate { get; set; }
        public TimeOnly Start { get; set; }
        public TimeOnly End { get; set; }
    }

    public class NewVenueBookingConfigurationDto
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
    }

    
}
