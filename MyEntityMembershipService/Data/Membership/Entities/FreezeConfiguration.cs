using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class FreezeConfiguration : BaseEntity, IBaseConfiguration
    {
        public int PackageId { get; set; }
        public int FreezeLimit { get; set; }
        public bool CanExceed { get; set; }
        public double PricePerAdditionalDay { get; set; }
        public PaymentProceduresType PaymentProceduresType { get; set; }

        public int UnitID { get; set; }
        public Unit Unit { get; set; }
    }
}
