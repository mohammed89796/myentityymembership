namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class SubMemberRemovalConfiguration : SharedEntity
    {
        public int RemovalConfigurationId { get; set; }
        public RemovalConfiguration RemovalConfiguration { get; set; }
        public int SubMemberId { get; set; }
        public MemberType MemberType { get; set; }
        public double? MembersRemovalFee { get; set; }
        public bool? MembersRemovalPermission { get; set; }
        public int UnitId { get; set; }
        public Unit Unit { get; set; }
    }
}
