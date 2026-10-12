namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class MembershipRequestBackgroundJobs :BaseEntity
    {
        public string BackgroundJobId { get; set; }
        public int MembershipRequestId { get; set; }
        public MembershipRequest MembershipRequest { get; set; }
    }
}
