using System.ComponentModel.DataAnnotations.Schema;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class BranchServices : BaseEntity
    {
        public int BranchID { get; set; }
        public int CoreServiceID { get; set; }

        [ForeignKey("BranchID")]
        public virtual Branch Branch { get; set; }

        [ForeignKey("CoreServiceID")]
        public virtual CoreService CoreService { get; set; } // Navigation to CoreService
    }
}
