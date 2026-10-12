namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class Form : BaseEntity
    {
        public string? Title { get; set; }
        public string? Alias { get; set; }
        public double? Fees { get; set; }
        public string? WorkflowSelection { get; set; }
        public int RequestTypeId { get; set; }
        public RequestType RequestType { get; set; }
        public List<FormMemberType>? MemberTypes { get; set; }
        public List<FormUserResponse> FormResponses { get; set; }
        public List<FormFields> FormFields { get; set; }
        public virtual ICollection<PaakageMemberForm>? PaakageMemberForms { get; set; } = new HashSet<PaakageMemberForm>();
        public virtual ICollection<NewMembershipConfiguration>? NewMembershipConfigurations { get; set; } = new HashSet<NewMembershipConfiguration>();
        public ICollection<AreaConfiguration> AreaConfigurations { get; set; } = new HashSet<AreaConfiguration>();
        public bool IsDraft { get; set; } = true;
        public int TenantId { get; set; }



        // self-referencing relationship for parent-child forms
        public int? ParentFormId { get; set; }
        public Form? ParentForm { get; set; }
        public ICollection<Form> ChildForms { get; set; } = [];
    }
}
