using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.PackageDtos
{
    public class PackageProductDto
    {
        public int ProductId { get; set; }

        public int? MaxSessions { get; set; }
        public SessionInterval? SessionInterval { get; set; }
        public int? PerInterval { get; set; }
    }
}
