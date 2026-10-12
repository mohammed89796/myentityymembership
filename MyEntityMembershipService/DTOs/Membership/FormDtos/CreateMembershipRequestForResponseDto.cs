using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.DTOs.Membership.FormDtos
{
    public class CreateMembershipRequestForResponseDto
    {
        public FormUserResponseDto FormUserResponseDto { get; set; }
        public int FormUserResponseId { get; set; }
        public Data.Membership.Entities.Membership Membership { get; set; }
        public Data.Membership.Entities.Membership SubMembership { get; set; }
        public Form Form {  get; set; }
        public List<FormUserResponseDto>? SubMembersFormUserResponseDto {  get; set; }
        public List<int>? SubMembersFormUserResponseId { get; set; }
    }
}
