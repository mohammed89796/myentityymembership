namespace MyEntityMembershipService.DTOs.Membership.FormDtos
{
    public class ReturnFormUserResponseDto
    {
        public int? Id { get; set; }
        public int FormId { get; set; }
        public string UserId { get; set; }
        public int? PackageId { get; set; }
        // submember user Id to be removed (in case of Removal Request) 
        public int? SubMemberId { get; set; }
        public int? MembershipId { get; set; }
        // Id Membership Request created for this response
        public int? MembershipRequestId { get; set; }
        public int? MemberTypeId { get; set; }
        public List<FieldResponseDto> FieldsResponse { get; set; }
        // For Case of creating New Membership Request at the same request Adding Members
        public List<FormUserResponseDto>? SubMembers { get; set; }

        // Added Section For Fees
        public List<FeesDto> FeesDto { get; set; }
    }
}
