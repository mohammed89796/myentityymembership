namespace MyEntityMembershipService.DTOs.Membership.FederationMmebrshipRequestsDtos
{
    public class MembershipRequestBranchAccessDto
    {
        public int BranchId { get; set; }
        public List<MembershipRequestFacilityAccessDto>? Facilities { get; set; }
    }

    public class MembershipRequestFacilityAccessDto
    {
        public int FacilityId { get; set; }
        public List<int>? VenueIds { get; set; }
    }
}

