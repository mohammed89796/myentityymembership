
using MyEntityMembershipService.Data.Membership.Entities.Enums;
using MyEntityMembershipService.DTOs.Membership.FormDtos;
using MyEntityMembershipService.DTOs.Membership.MembershipDtos;
using MyEntityMembershipService.DTOs.Membership.PaymentRequestDtos;
using MyEntityMembershipService.DTOs.Membership.RequestTypeDtos;


namespace MyEntityMembershipService.DTOs.Membership.FederationMmebrshipRequestsDtos
{
    public class GetEntityMembershipRequestDto
    {

        public int Id { get; set; }  
        public string UserId { get; set; }
        public List<GetRequestFieldResponseDto> FieldsResponse { get; set; }


    }


    public class GetRequestFieldResponseDto 
    {
        public int? FieldId { get; set; }

        public string FieldName { get; set; }
        public string? Response { get; set; }
    }
}
