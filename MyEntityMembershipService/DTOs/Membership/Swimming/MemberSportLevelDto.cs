using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.Swimming
{
    public class MemberSportLevelDto
    {
        public int MembershipId { get; set; }

        public string UserId { get; set; }

        public string Name { get; set; }

        public DateTime DateOfBirth { get; set; }

        public Gender Gender { get; set; }
        public string Sport { get; set; }
        public string SportNameAr {  get; set; }

        public string SportLevel { get; set; }

        public List<string> BranchNames { get; set; } = new();
    }
}
