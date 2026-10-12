using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.DTOs.Entity.BeltDtos;

namespace MyEntityMembershipService.DTOs.Entity.AtheleteDtos
{
    public class AtheleteToReturnDto
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }

        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public string Gender { get; set; }

        public DateOnly DateOfBirth { get; set; }
        public DateOnly DateOfAcquisition { get; set; }
        public string? Email { get; set; }
        public string MobileNumber { get; set; }

        public string? SchoolWorkEntity { get; set; }
        public string? PassportNumber { get; set; }

        public DateOnly DateOfExpiry { get; set; }
        public DateOnly? DateOfIssuance { get; set; }
        public string? NationalId { get; set; }

        public string HomeNumber { get; set; }

        public BeltToReturnDto Belt { get; set; }

        public string FileNumber { get; set; }
        public string MembershipStatus { get; set; }
        public int? BatchId { get; set; }
        public Batch Batch { get; set; }
        public string? PersonalPhotoUrl { get; set; }
        public string? PersonalPhotoWithStampUrl { get; set; }
        public string? NationalIdUrl { get; set; }
        public string? BeltCertificateUrl { get; set; }

        public int TenantId { get; set; }
    }
}
