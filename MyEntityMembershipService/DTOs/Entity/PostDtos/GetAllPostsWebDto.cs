namespace MyEntityMembershipService.DTOs.Entity.PostDtos
{
    public class GetAllPostsWebDto
    {
        public int TenantId { get; set; }

        public int PageNumber { get; set; } = 1;
        public string? SearchTerm { get; set; }
        public int PageSize { get; set; } = 10;
    }
}
