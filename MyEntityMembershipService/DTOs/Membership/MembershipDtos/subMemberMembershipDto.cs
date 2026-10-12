using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.MembershipDtos
{
    public class subMemberMembershipDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string MembershipType { get; set; }
        public string? MembershipStatus { get; set; }
        public string NationalID { get; set; }
        public string Nationality { get; set; }
        public string NationalIDPhoto { get; set; }
        public string UserPhoto { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public Gender Gender { get; set; }
        public string? Adress { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Governate { get; set; }
        public string Email { get; set; }

        public string UserID { get; set; }

        public int TenantId { get; set; }
        public int RequestId { get; set; }
        public int? ParentMembershipId { get; set; }

    }
}
