using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class Event : BaseEntity
    {

        public string EventName { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public string? Duration { get; set; }

        public string? Location { get; set; }

        public int? Price { get; set; }

        public Series Series { get; set; }

        public int SeriesId { get; set; }

        public ICollection<EventDetail> EventDetails { get; set; } = new HashSet<EventDetail>();

    }
}
