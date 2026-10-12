using System.ComponentModel.DataAnnotations.Schema;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class BranchImages : BaseEntity
    {
        public string ImageUrl { get; set; }
        public int BranchID { get; set; }

        [ForeignKey("BranchID")]
        public virtual Branch Branch { get; set; }
    }
}
