using System.ComponentModel.DataAnnotations.Schema;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class FacilityImages : BaseEntity
    {
        public string ImageUrl { get; set; }
        public int FacilityID { get; set; }

        [ForeignKey("FacilityID")]
        public virtual NewFacility Facility { get; set; }
    }
}
