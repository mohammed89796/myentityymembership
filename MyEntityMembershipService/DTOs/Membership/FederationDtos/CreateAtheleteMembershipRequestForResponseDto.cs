namespace MyEntityMembershipService.DTOs.Membership.FederationDtos
{
    public class CreateAtheleteMembershipRequestForResponseDto
    {
        public AddAtheleteFormResponseDto FormUserResponseDto { get; set; }
        public int? SuperMembershipRequestId { get; set; }
        public int FormUserResponseId { get; set; }
        public int TenantId { get; set; }
        public int? SuperTenentId { get; set; }
        public int BacthId { get; set; }
      

    }
}
