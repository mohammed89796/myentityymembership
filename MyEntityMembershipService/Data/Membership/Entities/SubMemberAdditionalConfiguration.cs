namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class SubMemberAdditionalConfiguration :SharedEntity
    {
        public int AdditionalConfigurationId { get; set; }
        public AdditionalConfiguration AdditionalConfiguration { get; set; }
        public int SubMemberId { get; set; }
        public MemberType MemberType { get; set; }
        public double Price { get; set; }
        public int UnitId { get; set; }
        public Unit Unit { get; set; }
    }
}
