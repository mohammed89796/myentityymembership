using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.AdditionalConfigurationDtos
{
    public class AdditionalConfigurationDto
    {
        public int? Id { get; set; }
        public int PackageId { get; set; }
        public PaymentProceduresType? PaymentProceduresType { get; set; }
        public List<AddSubMemberAdditionalConfigurationDto> SubMemberAdditionalConfigurations { get; set; }=new List<AddSubMemberAdditionalConfigurationDto>();
    }
}
