using MyEntityMembershipService.Data;
using MyEntityMembershipService.Data.Entity.Entities.Enums;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class Series : BaseEntity
    {
        public string SeriesName { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public int? Price { get; set; }

        public string? SeriesPictureUrl { get; set; }

        public RegistrationProcess? RegistrationProcess { get; set; }

        public RegistrationType? RegistrationType { get; set; }

        public string? AssignedAdmin { get; set; }

        public string? Description { get; set; }

        public ICollection<Event> Events { get; set; } = new HashSet<Event>();
    }
}
