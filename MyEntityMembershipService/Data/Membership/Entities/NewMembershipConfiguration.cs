using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class NewMembershipConfiguration : BaseEntity, IBaseConfiguration
    {
        public int PackageId { get; set; }
        public PaymentProceduresType PaymentProceduresType { get; set; }
        public int FormId { get; set; }
        public Form Form { get; set; }
    }
}
