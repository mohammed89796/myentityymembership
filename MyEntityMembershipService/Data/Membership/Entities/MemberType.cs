namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class MemberType : BaseEntity
    {
        public string Name { get; set; }
        public string ArabicName { get; set; }
        public List<FormMemberType> Forms { get; set; }

        public virtual ICollection<PaakageMemberForm> PaakageMemberForms { get; set; } = new HashSet<PaakageMemberForm>();

        public List<Membership> Memberships { get; set; }
        public List<MembershipRequest> MembershipRequests { get; set; }
        public List<FormUserResponse> FormUserResponses { get; set; }
        public ICollection<SubMemberAdditionalConfiguration> SubMemberAdditionalConfigurations { get; set; }
        public ICollection<SubMemberRemovalConfiguration> SubMemberRemovalConfigurations { get; set; }
        public ICollection <SubMemberCategory> SubMemberCategories { get; set; }= new HashSet<SubMemberCategory>();
        public ICollection<MemberRequestType> MemberRequestTypes { get; set; } = new HashSet<MemberRequestType>();

    }
}
