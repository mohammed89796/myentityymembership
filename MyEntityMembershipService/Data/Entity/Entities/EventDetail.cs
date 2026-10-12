using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class EventDetail : BaseEntity
    {
        public string Title { get; set; }

        public string Duration { get; set; }

        public string? Description { get; set; }

        public string? EventDetailPictureUrl { get; set; }

        public Event Event { get; set; }

        public int EventId { get; set; }

    }
}
