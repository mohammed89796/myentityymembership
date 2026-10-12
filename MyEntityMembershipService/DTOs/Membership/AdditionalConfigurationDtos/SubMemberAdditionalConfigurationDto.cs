
using MyEntityMembershipService.DTOs.Membership.FormDtos;

namespace MyEntityMembershipService.DTOs.Membership.AdditionalConfigurationDtos
{
    public class SubMemberAdditionalConfigurationDto
    {
        public int? AdditionalConfigurationId { get; set; }
        public int SubMemberId { get; set; }
        public MemberTypeDto? MemberType { get; set; }
        public double Price { get; set; }
        public int UnitId { get; set; }
    }
}
