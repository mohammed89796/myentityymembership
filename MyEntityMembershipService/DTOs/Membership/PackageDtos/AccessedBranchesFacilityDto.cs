

namespace MyEntityMembershipService.DTOs.Membership.PackageDtos
{
    public class AccessedBranchesFacilityDto
    {      
        public int FacilityId { get; set; }
        public List<AccessedFacilitiesVenueDto> VenueDtos { get; set; }

    }
}
