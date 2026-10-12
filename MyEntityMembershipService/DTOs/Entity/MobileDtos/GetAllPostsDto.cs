namespace MyEntityMembershipService.DTOs.Entity.MobileDtos
{
    public class GetAllPostsDto
    {
        public int TenantId { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string? SearchTerm { get; set; }

        public string UserId { get; set; }

    }
}
