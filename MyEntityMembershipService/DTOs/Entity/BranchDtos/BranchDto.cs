using MyEntityMembershipService.DTOs.Entity.FacilityDtos;

namespace MyEntityMembershipService.DTOs.Entity.BranchDtos
{
    public class BranchDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public int TenantId { get; set; }
        public string BranchImageURL { get; set; }
        public List<NewFacilityDto>? Facilities { get; set; } = new List<NewFacilityDto>();

    }
}
