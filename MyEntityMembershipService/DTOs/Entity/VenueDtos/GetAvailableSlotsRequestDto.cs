

using System.ComponentModel.DataAnnotations.Schema;

namespace MyEntityMembershipService.DTOs.Entity.VenueDtos
{
    public class GetAvailableSlotsRequestDto
    {
        public int VenueId { get; set; }
        public string Date { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
    public class AvailableSlotDto
    {
        public string SlotStart { get; set; }
        public string SlotEnd { get; set; }
        public string Status { get; set; }
        public string Price { get; set; }
    }
}
