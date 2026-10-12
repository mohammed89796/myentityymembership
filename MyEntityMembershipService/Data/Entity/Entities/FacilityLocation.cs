using System.ComponentModel.DataAnnotations.Schema;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class FacilityLocation : BaseEntity
    {
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public int FacilityID { get; set; }

        [ForeignKey("FacilityID")]
        public virtual NewFacility Facility { get; set; }
    }
}
