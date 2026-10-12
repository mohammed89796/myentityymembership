using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class PackageScope: BaseEntity
    {
        public PackageScopeType ScopeType { get; set; }
        public ICollection<PackageScopeAssignment> PackageScopeAssignments { get; set; } = [];
    }
}
