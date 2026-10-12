using MyEntityMembershipService.Data.Membership.Entities.Enums;
using MyEntityMembershipService.DTOs.Entity.PackageDtos;

namespace MyEntityMembershipService.DTOs.Membership.Swimming
{
    public class MembershipInjuryStatusDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string NationalId { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime? DateOfAcquisition { get; set; }
        public string ImageUrl { get; set; }
        public string Email { get; set; }
        public Gender Gender { get; set; }
        public List<MemberLevelDto> Levels { get; set; } = new();
        public string? MembershipNumber { get; set; }
        public DateTime? Issued { get; set; }
        public DateTime? Expiry { get; set; }
        public InjuryStatus InjuryStatus { get; set; }
        public List<string> BranchNames { get; set; } = new();



        public PackageSummaryDto? Package { get; set; }
    }
}
