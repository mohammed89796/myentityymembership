namespace MyEntityMembershipService.DTOs.Membership.MembershipDtos
{
    public class SportLevelTenantResponseDto
    {
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public string? TenantImageUrl { get; set; }
    }


    public class SportLevelTenantSearchDto
    {
        public string? SearchTerm { get; set; }
    }
}
