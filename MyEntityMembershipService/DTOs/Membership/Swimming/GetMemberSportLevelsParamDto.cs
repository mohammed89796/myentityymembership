using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.Swimming
{
    public class GetMemberSportLevelsParamDto
    {
        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string? SearchName { get; set; }

        public Gender? Gender { get; set; }

        public int? SportId { get; set; }

        public int? LevelId { get; set; }

        public DateTime? BirthDateFrom { get; set; }

        public DateTime? BirthDateTo { get; set; }

        public List<int>? BranchIds { get; set; }
    }
}
