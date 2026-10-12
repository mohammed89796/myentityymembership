namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class MembershipRequestProduct : BaseEntity
    {
        public int MembershipRequestId { get; set; }
        public MembershipRequest MembershipRequest { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

    }
}

