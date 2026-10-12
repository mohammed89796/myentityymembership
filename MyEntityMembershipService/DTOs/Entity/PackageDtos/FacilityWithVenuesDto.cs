using MyEntityMembershipService.DTOs.Entity.VenueDtos;

namespace MyEntityMembershipService.DTOs.Entity.PackageDtos
{
    public class FacilityWithVenuesDto
    {
        public int FacilityId { get; set; }
        public string FacilityName { get; set; }
        public string Image { get; set;}
        public List<NewVenueDto> Venues { get; set; } = new();
    }
}
