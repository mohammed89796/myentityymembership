using System.ComponentModel.DataAnnotations.Schema;
using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class NewVenueTypes : BaseEntity
    {
        public int VenueID { get; set; }

        [ForeignKey("VenueID")]
        public NewVenue? Venue { get; set; }
        public int TypeID { get; set; }

        [ForeignKey("TypeID")]
        public virtual VenueTypes VenueTypes { get; set; }
    }
}
