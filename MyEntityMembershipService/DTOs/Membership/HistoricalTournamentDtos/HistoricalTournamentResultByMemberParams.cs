using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.HistoricalTournamentDtos
{
    public class HistoricalTournamentResultByMemberParams
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int MembershipId { get; set; }
        public int SportId { get; set; }
        public List<int>? TournamentIds { get; set; }
        public List<int>? Races { get; set; }

        public int? RankFrom { get; set; }
        public int? RankTo { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string? SearchName { get; set; }
    }
}
