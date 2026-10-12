using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.Swimming
{
    public class GetSwimmingMembersParamDto
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public int? CategoryId { get; set; }

        public int? Status { get; set; }
        public int? SportId { get; set; }
        public int? LevelId { get; set; }
        public List<int>? BranchIds { get; set; }
        public string? SearchName { get; set; }       // Will search in full name, national ID, phone
        public InjuryStatus? HasInjury { get; set; }          // Filter for injury status
        public Gender? Gender { get; set; }
        public DateTime? BirthDateFrom { get; set; }
        public DateTime? BirthDateTo { get; set; }   
        public string? UserId { get; set; }
        public bool IsSuperAdmin { get; set; }
        public bool IsTenantAdmin { get; set; }
    }
}
