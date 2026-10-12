using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.ParentApplicationConfigurationDtos
{
    public class CreateParentApplicationConfigDto
    {
        public int? TenantId { get; set; }
        public int PackageId { get; set; }
        public PaymentProceduresType PaymentProceduresType { get; set; }
    }
}
