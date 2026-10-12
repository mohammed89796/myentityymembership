namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class PackageScopeAssignment : BaseEntity
    {
        public int PackageId { get; set; }
        public Package Package { get; set; }

        public int PackageScopeId { get; set; }
        public PackageScope PackageScope { get; set; }
    }
}
