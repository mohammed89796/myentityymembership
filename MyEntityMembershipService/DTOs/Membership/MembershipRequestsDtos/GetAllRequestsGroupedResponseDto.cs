using MyEntityMembershipService.DTOs.Membership.RequestTypeCategoryDtos;

namespace MyEntityMembershipService.DTOs.Membership.MembershipRequestsDtos
{
    public class GetAllRequestsGroupedResponseDto
    {
        public int TotalRequests { get; set; }
        public List<RequestTypeCategoryDto> GroupedRequests { get; set; }
    }

}