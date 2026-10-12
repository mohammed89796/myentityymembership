namespace MyEntityMembershipService.DTOs.Membership.RemovalConfigurationDtos
{
    public class RemovalConfigurationWithoutUnitDto
    {
        public int PackageId { get; set; }
        public int SubMemberId { get; set; }
        public double? MembersRemovalFee { get; set; }
        public bool? MembersRemovalPermission { get; set; }
    }
}
