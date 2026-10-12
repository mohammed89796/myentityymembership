

namespace MyEntityMembershipService.DTOs.Membership.RemovalConfigurationDtos
{
    public class SubMemberRemovalConfigurationDto
    {
        public int RemovalConfigurationId { get; set; }
        public int SubMemberId { get; set; }
        public string MemberTypeName { get; set; }
        public double? MembersRemovalFee { get; set; }
        public bool? MembersRemovalPermission { get; set; }
        public int UnitId { get; set; } = 4;
    }
}
