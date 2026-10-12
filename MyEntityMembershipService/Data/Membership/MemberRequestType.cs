using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Data.Membership
{
    public class MemberRequestType: BaseEntity
    {
        public int RequestTypeId { get; set; }
        public RequestType RequestType { get; set; }

        public int MemberTypeId { get; set; }
        public MemberType MemberType { get; set; }
    }
}
