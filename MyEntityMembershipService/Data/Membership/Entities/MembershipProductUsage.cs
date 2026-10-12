namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class MembershipProductUsage:BaseEntity
    {
        public int PackageId { get; set; }

        public Package Package { get; set; }
        public int ProductId { get; set; }

        public Product Product { get; set; }

        public int UsedSessions { get; set; }
        public int UsedEvents { get; set; }
    }
}
