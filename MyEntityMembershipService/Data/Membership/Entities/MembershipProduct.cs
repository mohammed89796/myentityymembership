using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class MembershipProduct : BaseEntity
    {
        public int MembershipId { get; set; }
        public Membership Membership { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public DateTime? ActivatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
