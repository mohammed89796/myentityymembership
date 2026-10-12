using MyEntityMembershipService.DTOs.Membership.FormDtos;

namespace MyEntityMembershipService.DTOs.Membership.FederationDtos
{
    public class ReturnAtheleteFormUserResponseDto
    {
        public int? BatchId { get; set; }
        public List<FieldResponseDto> FieldsResponse { get; set; }
       
    }
}
