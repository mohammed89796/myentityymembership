

namespace MyEntityMembershipService.DTOs.Membership.FormDtos
{
    public class ExecuteLogicForRequestTypeDto
    {
        public int FormUserResponseId { get; set; }
        public int RequestTypeId { get; set; }
        public int? SubMembershipRequestId { get; set; }
        public List<int>? SubMembershipRequestsId { get; set; }
    }
}
