using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class TurfTypes : BaseEntity
    {
        public string TypeName { get; set; }
        public ICollection<NewVenueTurfTypes>? VenueTurfTypes { get; set; }

    }
}
