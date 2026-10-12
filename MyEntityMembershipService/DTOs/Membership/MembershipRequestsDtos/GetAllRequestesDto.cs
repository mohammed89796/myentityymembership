namespace MyEntityMembershipService.DTOs.Membership.MembershipRequestsDtos
{
    public class GetAllRequestesDto
    {
        public int RequestTypeId { get; set; }
        public string RequestTypeName { get; set; }
        public int GroupTotalRequests { get; set; }
        public List<RequestStatusGroupDto> StatusGroups { get; set; }
    }

}
