using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.MembershipRequestsDtos
{
    public class UpdateRequestStatusWithComment
    {
        public int tenantId { get; set; }
        public int requestId {  get; set; }
        public int paymentId { get; set; }
        public RequestStatus newStatus { get; set; }
        public string comment { get; set; }
        public string? CreatedBy { get; set; }
    }
}
