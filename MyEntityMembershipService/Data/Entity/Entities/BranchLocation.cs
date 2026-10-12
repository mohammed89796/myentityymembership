using System.ComponentModel.DataAnnotations.Schema;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class BranchLocation : BaseEntity
    {
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public int BranchID { get; set; }

        [ForeignKey("BranchID")]
        public virtual Branch Branch { get; set; }
    }
}
