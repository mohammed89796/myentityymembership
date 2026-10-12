using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.MembershipRequestsDtos
{
    public class CreateMembershipRequestDto
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? NationalId { get; set; }
        public IFormFile? NationalIdImage { get; set; }
        public IFormFile? UserImage { get; set; }
        public int TenantId { get; set; }
        public string UserId { get; set; }
        public bool? IsTermsAgreed { get; set; }
        public string? Nationality { get; set; }
        public Gender? Gender { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Governate { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public int? PackageId { get; set; }
    }
}
