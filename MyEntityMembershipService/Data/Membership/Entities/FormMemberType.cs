namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class FormMemberType :SharedEntity
    {
        public int FormId { get; set; }
        public Form Form {  get; set; }
        public int MemberTypeId { get; set; }
        public MemberType MemberType { get; set; }
    }
}
