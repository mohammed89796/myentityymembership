using MyEntityMembershipService.Data.Entity.Entities.Enums;
using MyEntityMembershipService.DTOs.Entity.TenantDtos;

namespace MyEntityMembershipService.DTOs.Entity.PackageDtos
{
    public class PackageDataDto
    {
        public string Name { get; set; }
        public int? Duration { get; set; }
        public DurationUnit? DurationUnit { get; set; }
        public double Price { get; set; }
        public int TenantId { get; set; }
        public int? SubMembersLimit { get; set; }
        public TenantDto? Tenant { get; set; }
    }
}
