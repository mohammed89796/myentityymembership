

namespace MyEntityMembershipService.DTOs.Membership.PackageRequestAuthorizationDtos
{
    public class PaakageMemberFormDto
    {
        public int PackageId { get; set; }
        public int FormId { get; set; }
        public double? Fees { get; set; }

        public int MemberTypeId { get; set; }
    }
}
