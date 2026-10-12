using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.RenewalConfigurationDtos
{
    public class RenewalConfigurationDto
    {
        public int PackageId { get; set; }
        public bool? AutoRenewal { get; set; }
        public bool? EarlyRenew { get; set; }
        public bool? FormResubmission { get; set; }
        public bool? RenewalLimit { get; set; }
        public int? RenewalLimitNumber { get; set; }
        public PaymentProceduresType? PaymentProceduresType { get; set; }
        public List<RenwalOptionDto>? RenewalOptions { get; set; }
    }
}
