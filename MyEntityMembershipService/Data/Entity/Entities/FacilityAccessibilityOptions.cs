using System.ComponentModel.DataAnnotations.Schema;
using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class FacilityAccessibilityOptions : BaseEntity
    {
        public int FacilityID { get; set; }
        public int AccessibilityOptionID { get; set; }

        [ForeignKey("FacilityID")]
        public virtual NewFacility Facility { get; set; }

        [ForeignKey("AccessibilityOptionID")]
        public virtual AccessibilityOptions AccessibilityOptions { get; set; }
    }
}
