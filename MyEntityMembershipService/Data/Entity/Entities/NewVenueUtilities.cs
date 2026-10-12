using System.ComponentModel.DataAnnotations.Schema;
using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class NewVenueUtilities : BaseEntity
    {
        public int VenueID { get; set; }

        [ForeignKey("VenueID")]
        public NewVenue? Venue { get; set; }
        public int UtilityID { get; set; }

        [ForeignKey("UtilityID")]
        public virtual NewCoreUtilities VenueUtilities { get; set; }
    }
}
