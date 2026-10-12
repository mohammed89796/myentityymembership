namespace MyEntityMembershipService.DTOs.Membership.MembershipDtos
{
    public class GetFederationMembersParamDto
    {
        public int? BranchId { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        // For Filter
        public int? Status { get; set; }
        public string? BeltName { get; set; }
        public string? SearchName { get; set; }
    }
}
