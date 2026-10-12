namespace MyEntityMembershipService.DTOs.Membership.PackageRequestAuthorizationDtos
{
    public class PackageMemberTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<PackageRequestTypeDto> RequestTypes { get; set; } = new List<PackageRequestTypeDto>();
    }
}
