using MyEntityMembershipService.DTOs.Membership.PackageRequestAuthorizationDtos;

namespace MyEntityMembershipService.DTOs.Membership.FormDtos
{
    public class MembershipAuthorizationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<AuthorizationMemberTypesDto> MemberTypes { get; set; }
    }
}
