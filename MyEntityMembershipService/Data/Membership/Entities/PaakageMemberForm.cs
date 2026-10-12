namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class PaakageMemberForm:SharedEntity
    {
        public int PackageId { get; set; }
        public virtual Package? Package { get; set; }
        public int FormId { get; set; }
        public double? Fees { get; set; }
        public virtual Form? Form { get; set; }
        public int MemberTypeId { get; set; }
        public virtual MemberType? MemberType { get; set; }
    }
}
