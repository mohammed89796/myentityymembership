using System.ComponentModel.DataAnnotations.Schema;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class VenueLocation : BaseEntity
    {
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public int VenueID { get; set; }

        [ForeignKey("VenueID")]
        public virtual NewVenue Venue { get; set; }
    }
}
