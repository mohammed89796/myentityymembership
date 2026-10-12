using System.ComponentModel.DataAnnotations.Schema;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class FacilityServices : BaseEntity
    {
        public int FacilityID { get; set; }
        public int CoreFacilityServiceID { get; set; }

        [ForeignKey("FacilityID")]
        public virtual NewFacility Facility { get; set; }

        [ForeignKey("CoreFacilityServiceID")]
        public virtual CoreFacilityServices CoreFacilityServices { get; set; }
    }
}
