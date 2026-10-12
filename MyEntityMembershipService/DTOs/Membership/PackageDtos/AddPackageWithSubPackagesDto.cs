using MyEntityMembershipService.Data.Entity.Entities.Enums;
using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.PackageDtos
{
    public class AddPackageWithSubPackagesDto
    {
        public string Name { get; set; }
        public int Duration { get; set; }
        public decimal Price { get; set; }
        public IFormFile ImageUrl { get; set; }

        public List<PackageScopeType> PackageScopeTypes { get; set; } = new();

        public List<int> VenueIds { get; set; } = new();

        public List<PackageProductDto>? Products { get; set; }
        public PackageType? Visibility { get; set; }
        public MembershipMode IsMainMember { get; set; }

        public List<SubMemberCategoryDto>? SubMemberCategories { get; set; }

        public List<int>? RequestAuthorizationIds { get; set; }

        public List<SubPackageDto>? SubPackages { get; set; }
    }
}
