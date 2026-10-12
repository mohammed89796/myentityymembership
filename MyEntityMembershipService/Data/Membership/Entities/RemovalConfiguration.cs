using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class RemovalConfiguration : BaseEntity, IBaseConfiguration
    {
        public int PackageId { get; set; }
        public PaymentProceduresType PaymentProceduresType { get; set; }

        public ICollection<SubMemberRemovalConfiguration> SubMemberRemovalConfigurations { get; set; }
    }
}
