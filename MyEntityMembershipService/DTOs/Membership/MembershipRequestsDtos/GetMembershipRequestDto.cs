
using MyEntityMembershipService.DTOs.Membership.FormDtos;
using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.MembershipRequestsDtos
{
    public class GetMembershipRequestDto
    {
        public int Id { get; set; }
        public int Status { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? NationalId { get; set; }
        public string? NationalIdPhoto { get; set; }
        public string? UserPhoto { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public string UserId { get; set; }
        public string MembershipId { get; set; }
        public int? RequestTypeId { get; set; }
        public string? CommentsAndJustfications { get; set; }
        public bool? IsTermsAgreed { get; set; }
        public string? Nationality { get; set; }
        public Gender? Gender { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Governate { get; set; }
        public int? PackageId { get; set; }
        public double? FormFees { get; set; }
        public DateTime? LastStatusUpdate { get; set; }
        public int? MemberTypeId { get; set; }
        public DateTime? CreationDate { get; set; }
        public string? MemberTypeName { get; set; }
        public FormUserResponseDto FormUserResponseDto { get; set; }
        public List<SubMemberDataDto>? SubMembers { get; set; }
        public List<DocumentSubmitted> DocumentsSubmitted { get; set; } = new List<DocumentSubmitted>();

    }
}
