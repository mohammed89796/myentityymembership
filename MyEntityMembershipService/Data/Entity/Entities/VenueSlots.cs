using System.ComponentModel.DataAnnotations.Schema;
using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class VenueSlots : BaseEntity
    {
        public int VenueId { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsBooked { get; set; } = false;
        public string? BookedBy { get; set; }

    }
}
