namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class FormUserResponse :BaseEntity
    {
        public string? UserId { get; set; }
        public string? BatchId { get; set; }
        public int? PackageId { get; set; }
        public int FormId { get; set; }
        public Form Form { get; set; }
        public int? MembershipId { get; set; }
        public Membership Membership { get; set; }
        public int? SubMemberId { get; set; }
        public Membership SubMemberMembership { get; set; }
        public int? MembershipRequestId {  get; set; }
        public MembershipRequest MembershipRequest { get; set; }
        public List<FieldResponse> FieldsResponse { get; set; }
        public int? MemberTypeId { get; set; }
        public MemberType MemberType { get; set; }
    }
}
