using MyEntityMembershipService.DTOs.Entity.BranchDtos;
using MyEntityMembershipService.DTOs.Entity.FacilityDtos;
using MyEntityMembershipService.DTOs.Entity.VenueDtos;

namespace MyEntityMembershipService.DTOs.Membership.FederationMmebrshipRequestsDtos
{
    public class MembershipRequestAccessTreeDto
    {
        public IEnumerable<int> SelectedBranches { get; set; }
        public IEnumerable<int> SelectedFacilities { get; set; }
        public IEnumerable<int> SelectedVenues { get; set; }
    
}

   

}
