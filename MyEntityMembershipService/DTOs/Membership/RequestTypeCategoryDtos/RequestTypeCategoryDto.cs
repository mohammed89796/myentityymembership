using MyEntityMembershipService.DTOs.Membership.MembershipRequestsDtos;

namespace MyEntityMembershipService.DTOs.Membership.RequestTypeCategoryDtos
{
    public class RequestTypeCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<GetAllRequestesDto> RequestTypes { get; set; }
    }
}
