namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class RequestType : BaseEntity
    {
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string Description { get; set; }
        public int RequestTypeCategoryId { get; set; }
        public virtual RequestTypeCategory RequestTypeCategory { get; set; }
        public virtual ICollection<MembershipRequest> MembershipRequests { get; set; }
        public List<Form> Forms { get; set; }
        public List<RequestTypeFields> Fields { get; set; }
        public virtual ICollection<PackageRequestAuthourizations> PackageRequestAuthourizations { get; set; } = new HashSet<PackageRequestAuthourizations>();
        public virtual Configuration Configuration { get; set; }
        public ICollection<AreaConfiguration> AreaConfigurations { get; set; } = [];
        public ICollection<Batch> Batches { get; set; } = [];
        public ICollection<MemberRequestType> MemberRequestTypes { get; set; } = new HashSet<MemberRequestType>();


    }
}
