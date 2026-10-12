using System.ComponentModel.DataAnnotations;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class AccessedBranchesFacilities
    {
        [Key]
        public int PackageId { get; set; }
        public Package Package { get; set; }

        [Key]
        public int BranchId { get; set; }
        public Branch Branch { get; set; }

        [Key]

        public int FacilityId { get; set; }
        public NewFacility Facility { get; set; }

        //public Facility Facility { get; set; }

    }
}
