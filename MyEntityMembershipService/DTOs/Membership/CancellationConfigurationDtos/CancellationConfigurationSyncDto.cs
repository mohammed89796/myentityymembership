

using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.CancellationConfigurationDtos
{
    public class CancellationConfigurationSyncDto
    {
        public int PackageId { get; set; }
        public bool Refund { get; set; }
        public bool None { get; set; }
        public bool Refund25Percent { get; set; }
        public bool Refund50Percent { get; set; }
        public bool Refund75Percent { get; set; }
        public bool Refund100Percent { get; set; }
        public int RefundDuration { get; set; }
        public int UnitId { get; set; }
        public PaymentProceduresType? PaymentProceduresType { get; set; }
    }
}
