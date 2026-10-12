using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class Batch : BaseEntity
    {
        public string BatchNumber { get; set; }
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public string TenantName { get; set; }
        public int? SuperTenentId { get; set; }
        public string? SuperEntityName { get; set; }
        public int? PackageId { get; set; }
        public int? PackagePrice { get; set; }
        public int RequestTypeId { get; set; }
        public RequestType RequestType { get; set; }

        public BatchStatus Status { get; set; }
        public ICollection<MembershipRequest> MembershipRequests { get; set; } = new List<MembershipRequest>();
        public ICollection<PaymentBatch> PaymentBatches { get; set; } = new HashSet<PaymentBatch>();
        public ICollection<NewPaymentRequest> NewPaymentRequests { get; set; } = new HashSet<NewPaymentRequest>();
    }
}
