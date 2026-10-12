using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.ParentApplicationConfigurationDtos
{
    public class ParentApplicationConfigResponseDto
    {
        public int Id { get; set; }
        public int PackageId { get; set; }
        public int FormId { get; set; }
        public PaymentProceduresType PaymentProceduresType { get; set; }
    }
}
