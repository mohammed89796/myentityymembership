using MyEntityMembershipService.Data.Membership.Entities.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class AdditionalConfiguration : BaseEntity ,IBaseConfiguration
    {
        public int PackageId { get; set; }
        public PaymentProceduresType PaymentProceduresType { get; set; }
        public ICollection<SubMemberAdditionalConfiguration> SubMemberAdditionalConfigurations { get; set; }
    }
}
