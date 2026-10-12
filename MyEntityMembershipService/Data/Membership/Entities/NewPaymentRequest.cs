using MyEntityMembershipService.Data.Entities.Enums;
using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class NewPaymentRequest : BaseEntity
    {
        public PaymentBatch PaymentBatch { get; set; }

        public int? PaymentBatchId { get; set; }

        public int BatchId { get; set; }
        public Batch Batch { get; set; }

        public int EntityId { get; set; }

        public Tenant Entity { get; set; }
        public int? SuperEntityId { get; set; }

        public Tenant? SuperEntity { get; set; }
        public string? EntityName { get; set; }
        public string? SuperEntityName { get; set; }
        public MembershipRequest MembershipRequest { get; set; }
        public int MembershipRequestId { get; set; }
        public PaymentStatus PaymentStatus { get; set; }

        public BranchAdminStatus BranchAdminStatus { get; set; }

        public OperationType OperationType { get; set; }
    }
}
