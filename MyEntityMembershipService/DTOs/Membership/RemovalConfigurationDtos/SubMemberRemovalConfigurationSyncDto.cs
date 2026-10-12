namespace MyEntityMembershipService.DTOs.Membership.RemovalConfigurationDtos
{
    public class SubMemberRemovalConfigurationSyncDto
    {
        //public int RemovalConfigurationId { get; set; }
        public int SubMemberId { get; set; }
        public double? MembersRemovalFee { get; set; }
        public bool? MembersRemovalPermission { get; set; }
        public int UnitId { get; } = 4;
    }
}
