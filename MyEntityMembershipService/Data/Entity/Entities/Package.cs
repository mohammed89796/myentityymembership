using MyEntityMembershipService.Data.Entity.Entities.Enums;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class Package : BaseEntity
    {
        public string Name { get; set; }
        public int? Duration { get; set; }
        public DurationUnit? DurationUnit { get; set; }
        public double Price { get; set; }
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public string? Image { get; set; }
        public int? SuperPackageId { get; set; }

        public Package? SuperPackage { get; set; }
        public Enums.PackageType? PackageType { get; set; }
        public int? SubMembersLimit { get; set; }
        public bool IsActive { get; set; } = false;
        public Membership.Entities.Enums.PackageScopeEnum? PackageScope { get; set; }
        public MembershipMode MembershipMode { get; set; }
        public ParentApplicationConfiguration? ParentApplicationConfiguration { get; set; }
        public ICollection<Package> SubPackages { get; set; }  // children
        public ICollection<SubMemberCategory>? SubMemberCategories { get; set; } = new List<SubMemberCategory>();

        public ICollection<AccessedBranches>? Branches { get; set; } = new List<AccessedBranches>();
        public ICollection<AccessedBranchesFacilities>? Facilities { get; set; } = new List<AccessedBranchesFacilities>();

        public ICollection<AccessedFacilitiesVenues>? Venues { get; set; } = new List<AccessedFacilitiesVenues>();
        public ICollection<PaakageMemberForm> PackageMemberForms { get; set; } = new List<PaakageMemberForm>();
        public ICollection<Data.Membership.Entities.Membership> Memberships { get; set; } = new HashSet<Data.Membership.Entities.Membership>();
        public ICollection<MembershipRequest> MembershipRequests { get; set; }=new HashSet<MembershipRequest>();

        public ICollection<PackageProduct> PackageProducts { get; set; } = new List<PackageProduct>();
        public ICollection<PackageScopeAssignment> PackageScopeAssignments { get; set; }= [];
        public ICollection<PackageRequestAuthourizations> PackageRequestAuthourizations { get; set; } = new HashSet<PackageRequestAuthourizations>();      

    }
}
