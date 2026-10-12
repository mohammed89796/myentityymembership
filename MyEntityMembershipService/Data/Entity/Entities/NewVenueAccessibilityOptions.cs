using System.ComponentModel.DataAnnotations.Schema;
using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class NewVenueAccessibilityOptions : BaseEntity
    {
        public int VenueID { get; set; }

        [ForeignKey("VenueID")]
        public NewVenue? Venue { get; set; }
        public int AccessibilityOptionID { get; set; }

        [ForeignKey("AccessibilityOptionID")]
        public virtual NewAccessibilityOptions AccessibilityOptions { get; set; }
    }
}
