using MyEntityMembershipService.Data.Entity.Entities.Enums;
using MyEntityMembershipService.Data.Membership.Entities.Enums;
using MyEntityMembershipService.DTOs.Entity.PackageDtos;

public class AddPackageDto
{
    public string Name { get; set; }
    public int? Duration { get; set; }
    public DurationUnit? DurationUnit { get; set; }
    public double Price { get; set; }
    public int TenantId { get; set; }
    public PackageType? PackageType { get; set; }
    public PackageScopeEnum? Features { get; set; }
    public bool? IsMainMember { get; set; }
    public int? SubMembersLimit { get; set; }
    public List<AccessedBranchesDto>? Branches { get; set; }
    public List<SubMemberCategoryDto>? SubMemberCategories { get; set; } = new List<SubMemberCategoryDto>();
    public List<CreatePackageAuthourizationsDto>? PackageAuthourizations { get; set; }= new List<CreatePackageAuthourizationsDto>();
}
