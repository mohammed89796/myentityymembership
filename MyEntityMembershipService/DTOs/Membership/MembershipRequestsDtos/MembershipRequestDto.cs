
using MyEntityMembershipService.Data.Membership.Entities.Enums;
using MyEntityMembershipService.DTOs.Membership.FormDtos;
using MyEntityMembershipService.DTOs.Membership.MembershipDtos;
using MyEntityMembershipService.DTOs.Membership.PaymentRequestDtos;
using MyEntityMembershipService.DTOs.Membership.RequestTypeDtos;


namespace MyEntityMembershipService.DTOs.Membership.MembershipRequestsDtos
{
    public class MembershipRequestDto
    {

        public int Id { get; set; }
        public RequestStatus Status { get; set; }
        public DateTime? LastStatusUpdate { get; set; }

        public DateTime? ExpiryDate { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string NationalId { get; set; }
        public string NationalIdPhoto { get; set; }
        public string UserPhoto { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string SecondPhoneNumber { get; set; }
        public string SecondEmail { get; set; }
        public int? BuildUpId { get; set; }
        public int TenantId { get; set; }
        public string? TenantName { get; set; }


        public string SeparationReason { get; set; }
        public int MembershipType { get; set; }
        public double? FormFees  { get; set; }
        //public TenantDto TenantDto { get; set; }
        public string UserId { get; set; }
        public int? MembershipId { get; set; }
        public MembershipDto Membership { get; set; }
        public RequestTypeDto RequestType { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public string? CommentsAndJustfications { get; set; }
        public bool? IsTermsAgreed { get; set; }
        public string? Nationality { get; set; }
        public Gender? Gender { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Governate { get; set; }
        public List<PaymentRequestDto> PaymentRequestDto { get; set; }=new List<PaymentRequestDto>();
        public int? SuperMembershipId { get; set; }
        public int? SubMemberCategoryId { get; set; }
        public int? PackageId { get; set; }
        public int? MemberTypeId { get; set; }
        public MemberTypeDto? MemberType { get; set; }
        public GetFormUserResponseDto FormUserResponse { get; set; }
        public List<SubMemberMembershipRequestDto>? SubMembers { get; set; } = new List<SubMemberMembershipRequestDto>();


    }
}
