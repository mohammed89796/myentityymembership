

using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.FederationDtos
{
    public class PindingRequestsDto
    {
        public int BatchId { get; set; }

        public int EntityId { get; set; }
        public int SuperEntityId { get; set; }
        public List<int> MemberRequestsIds { get; set; } = new List<int>();

        public OperationType OperationType { get; set; }
    }
}
