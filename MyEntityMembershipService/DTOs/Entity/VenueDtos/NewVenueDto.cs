
using System.ComponentModel.DataAnnotations.Schema;

namespace MyEntityMembershipService.DTOs.Entity.VenueDtos
{
    public class NewVenueDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string VenueImageURL { get; set; }
        public string? Description { get; set; }
        public int FacilityId { get; set; }

    }
}
