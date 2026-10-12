using MyEntityMembershipService.Data.Entity.Entities.Enums;
using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.PackageDtos
{
    public class GetAllPackageResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int? Duration { get; set; }
        public string? Image { get; set; }
        public PackageType? PackageType { get; set; }
        public ICollection<PackageScopeType> PackageScopes { get; set; } = [];
        public int MembersCount { get; set; }
    }


    public class PackageResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public bool IsActive { get; set; }
        public int? Duration { get; set; }
        public string? Image { get; set; }
        public PackageType? PackageType { get; set; }
        public ICollection<PackageScopeType> PackageScopes { get; set; } = [];
    }


    public class GetAllPackageFilterResponseDto
    {
        public int? TenantId { get; set; }
        public string? SearchTerm { get; set; }
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;

    }
}
