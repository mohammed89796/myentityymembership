

namespace MyEntityMembershipService.DTOs.Entity.PackageDtos
{
    public class AccessedBranchesDto
    {
        public int BranchId { get; set; }
        public List<AccessedBranchesFacilityDto> FacilityDtos { get; set; }

    }
}
