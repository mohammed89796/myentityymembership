namespace MyEntityMembershipService.DTOs.Membership.PackageDtos
{
    public class UpdatePackageActivitionDto
    {
        public int? TenantId { get; set; }
        public int PackageId { get; set; }
        public bool IsActive { get; set; }
    }
}
