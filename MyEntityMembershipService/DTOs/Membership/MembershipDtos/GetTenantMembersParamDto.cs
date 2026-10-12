namespace MyEntityMembershipService.DTOs.Membership.MembershipDtos
{
    public class GetTenantMembersParamDto
    {
        public int TenantId { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        // For Filter
        public int? Status {  get; set; }
        public string? BeltName { get; set; }
        public string? SearchName { get; set; }
    }
}
