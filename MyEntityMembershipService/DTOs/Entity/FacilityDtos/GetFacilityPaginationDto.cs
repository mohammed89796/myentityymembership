

namespace MyEntityMembershipService.DTOs.Entity.FacilityDtos
{
    public class GetFacilityPaginationDto
    {
        public int totalPages { get; set; }
        public int currentPage { get; set; }
        public List<Facility> facilities { get; set; }
    }
}
