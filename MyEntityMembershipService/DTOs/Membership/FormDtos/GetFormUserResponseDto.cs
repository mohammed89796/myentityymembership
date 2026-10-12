
using MyEntityMembershipService.DTOs.Membership.MembershipDtos;

namespace MyEntityMembershipService.DTOs.Membership.FormDtos
{
    public class GetFormUserResponseDto
    {
        public string UserId { get; set; }
        public int FormId { get; set; }
        public double? FormFees { get; set; }

        public FormDto? Form { get; set; }
        public int? MembershipId { get; set; }
        public MembershipDto Membership { get; set; }
        public int? SubMemberId { get; set; }
        public MembershipDto SubMemberMembership { get; set; }
        public int? MembershipRequestId { get; set; }
        public List<GetFieldResponseDto> FieldsResponse { get; set; }
    }
}
