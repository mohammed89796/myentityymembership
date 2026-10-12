using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.RemovalConfigurationDtos
{
    public class RemovalConfigurationSyncDto
    {
        public int PackageId { get; set; }
        public PaymentProceduresType? PaymentProceduresType { get; set; }
        public ICollection<SubMemberRemovalConfigurationSyncDto> SubMemberRemovalConfigurationSyncDtos { get; set; } = new List<SubMemberRemovalConfigurationSyncDto>();
    }
}
