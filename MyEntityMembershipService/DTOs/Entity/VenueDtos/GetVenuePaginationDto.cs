

namespace MyEntityMembershipService.DTOs.Entity.VenueDtos
{
    public class GetVenuePaginationDto
    {
        public int totalPages { get; set; }
        public int currentPage { get; set; }
        public List<Venue> venues { get; set; }
    }
}
