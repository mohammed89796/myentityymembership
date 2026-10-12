using System.ComponentModel.DataAnnotations;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class AccessedBranches
    {
        [Key]

        public int PackageId { get; set; }
        public Package Package { get; set; }
        [Key]

        public int BranchId { get; set; }
        public Branch Branch { get; set; }



    }
}
