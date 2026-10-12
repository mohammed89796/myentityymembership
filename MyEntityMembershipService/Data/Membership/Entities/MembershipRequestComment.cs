using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class MembershipRequestComment : BaseEntity
    {
        public string CommentsAndJustfications { get; set; }
        public RequestStatus RequestStatus { get; set; }
        public int MembershipRequestId { get; set; }
        public MembershipRequest MembershipRequest { get; set; }
    }
}
