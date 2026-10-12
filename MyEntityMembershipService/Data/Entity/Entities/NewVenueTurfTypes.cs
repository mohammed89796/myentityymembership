using System.ComponentModel.DataAnnotations.Schema;
using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class NewVenueTurfTypes : BaseEntity
    {
        public int VenueID { get; set; }

        [ForeignKey("VenueID")]
        public NewVenue? Venue { get; set; }
        public int TurfTypeID { get; set; }

        [ForeignKey("TurfTypeID")]
        public virtual TurfTypes VenueTurfTypes { get; set; }
    }
}
