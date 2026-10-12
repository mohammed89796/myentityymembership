using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.MembershipRequestsDtos
{
    public class ActivityDto
    {
        public string Comments { get; set; }
        public RequestStatus RequestStatus { get; set; }
        public int MembershipRequestId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreationDate { get; set; }
    }
}
