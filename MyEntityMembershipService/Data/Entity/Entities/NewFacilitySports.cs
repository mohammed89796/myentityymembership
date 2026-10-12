using System.ComponentModel.DataAnnotations.Schema;
using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class NewFacilitySports : BaseEntity
    {
        public int FacilityID { get; set; }
        public int SportID { get; set; }

        [ForeignKey("FacilityID")]
        public virtual NewFacility Facility { get; set; }

        [ForeignKey("SportID")]
        public virtual Sports Sports { get; set; }
    }
}
