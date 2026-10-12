using MyEntityMembershipService.Data.Entity.Entities.Enums;
using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.PackageDtos
{
    public class SubPackageResponsDto
    {
        public int Id { get; set; }
        public int SuperPackageId { get; set; }
        public string Name { get; set; }
        public int? Duration { get; set; }
        public PackageType? PackageType { get; set; }
        public ICollection<PackageScopeType> PackageScopes { get; set; } = [];
        public ICollection<SubPackageProduct>? ProductDetails { get; set; }
        public ICollection<SubPackageProductType>? ProductTypes { get; set; }
    }

    public class SubPackageProduct
    {
        public int? MaxSessions { get; set; }
        public SessionInterval? SessionInterval { get; set; }
        public int? PerInterval { get; set; }
    }


    public class SubPackageProductType
    {
        public int ProductId { get; set; }
        public ProductType ProductType { get; set; }
        public string Name { get; set; } = null!;
    }
}
