
using MyEntityMembershipService.Data.Entity.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.PackageDtos
{
    public class PackageDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Duration { get; set; }
        public double Price { get; set; }
        public int TenantId { get; set; }
        public int SubMembersLimit { get; set; }
        public string Image { get; set; }
        public DateTime? CreationDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModificationDate { get; set; }

        public List<SubMemberCategoryDto>? SubMemberCategories { get; set; } = new List<SubMemberCategoryDto>();
    
        public List<PackageAuthourizationsDto>? PackageAuthourizations { get; set; } = new List<PackageAuthourizationsDto>();

    }
}
