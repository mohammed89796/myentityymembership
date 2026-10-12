

using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.RemovalConfigurationDtos
{
    public class RemovalConfigurationDto
    {
        public int Id { get; set; }
        public int PackageId { get; set; }
        public PaymentProceduresType PaymentProceduresType { get; set; }
        public ICollection<SubMemberRemovalConfigurationDto> SubMemberRemovalConfigurationDtos { get; set; }

    }
}
