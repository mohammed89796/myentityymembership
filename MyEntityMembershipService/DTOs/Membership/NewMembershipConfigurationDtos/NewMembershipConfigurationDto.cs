

using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.NewMembershipConfigurationDtos
{
    public class NewMembershipConfigurationDto
    {
        public int PackageId { get; set; }
        public int FormID { get; set; }

        public PaymentProceduresType? PaymentProceduresType { get; set; }
    }
}
