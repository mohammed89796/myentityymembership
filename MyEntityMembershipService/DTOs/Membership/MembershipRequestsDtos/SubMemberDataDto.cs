using MyEntityMembershipService.DTOs.Membership.FormDtos;

namespace MyEntityMembershipService.DTOs.Membership.MembershipRequestsDtos
{
    public class SubMemberDataDto
    {
        public SubMemberDto SubMemberDto { get; set; }
        public FormUserResponseDto FormUserResponseDto { get; set; }
        public List<DocumentSubmitted> DocumentsSubmitted { get; set; } = new List<DocumentSubmitted>();
    }
}
