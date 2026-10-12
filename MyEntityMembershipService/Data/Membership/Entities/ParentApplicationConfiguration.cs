using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class ParentApplicationConfiguration:BaseEntity , IBaseConfiguration
    {
        public int PackageId { get; set; }
        public Package Package { get; set; }
        public PaymentProceduresType PaymentProceduresType { get; set; }
    }
}
