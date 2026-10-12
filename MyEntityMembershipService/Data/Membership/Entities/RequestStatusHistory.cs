using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class RequestStatusHistory:SharedEntity
    {
        public int MembershipRequestId { get; set; }
        public RequestStatus RequestStatusId{ get; set;}
        public virtual MembershipRequest MembershipRequest { get; set; }
    }
}
