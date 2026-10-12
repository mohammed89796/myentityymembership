using MyEntityMembershipService.Data.Membership.Entities.Enums;
using System.Data;

namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class RenewalConfiguration : BaseEntity, IBaseConfiguration
    {
        public int PackageId { get; set; }
        public PaymentProceduresType PaymentProceduresType { get; set; }
        public bool? AutoRenewal { get; set; }
        public bool? EarlyRenew { get; set; }
        public bool? FormResubmission { get; set; }
        public bool? RenewalLimit { get; set; }
        public int? RenewalLimitNumber { get; set; }
        public virtual ICollection<RenwalOption> RenewalOptions { get; set; }=new HashSet<RenwalOption>();

    }
}
