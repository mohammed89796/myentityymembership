using MyEntityMembershipService.Data.Membership.Entities.Enums;
using MyEntityMembershipService.DTOs.Membership.FormDtos;

namespace MyEntityMembershipService.DTOs.Membership.MembershipRequestsDtos
{
    public class CurrentMembershipDto
    {
        public int Id { get; set; }
        public string? TenantName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public MembershipStatus? MembershipStatus { get; set; }
        public string? UserPhoto { get; set; }
        public int TenantId { get; set; }
        public string? TenantImage { get; set; }
        public int? PackageId { get; set; }
        public string? PackageName { get; set; }
        public List<MembershipAuthorizationDto> MembershipAuthorizations { get; set; }
    }
}
