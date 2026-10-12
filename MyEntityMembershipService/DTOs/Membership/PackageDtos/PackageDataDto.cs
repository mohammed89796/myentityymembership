namespace MyEntityMembershipService.DTOs.Membership.PackageDtos
{
    public class PackageDataDto
    {
        public string Name { get; set; }
        public int? Duration { get; set; }
        public int? DurationUnit { get; set; }
        public double Price { get; set; }
        public int TenantId { get; set; }
        public int? SubMembersLimit { get; set; }
    }
}
