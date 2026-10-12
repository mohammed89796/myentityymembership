

namespace MyEntityMembershipService.DTOs.Entity.OrganizationsDtos
{
    public class GetOrganizationPaginationDto
    {
        public int totalPages { get; set; }
        public int currentPage { get; set; }
        public List<OrganizationEntity> organizations { get; set; }
    }
}
