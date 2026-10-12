namespace MyEntityMembershipService.DTOs.Membership.MembershipDtos
{
    public class SportLevelsResponseDto
    {
        public int SportId { get; set; }
        public string SportName { get; set; }
        public string SportNameAr { get; set; }
        public string? SportImageUrl { get; set; }
        public DateTime? CreationDate { get; set; }
        public int TenantId { get; set; }
    }


    public class SportLevelSearchDto
    {
        public string? SearchTerm { get; set; }
    }
}
