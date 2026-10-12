using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.HistoricalTournamentDtos
{
    public class GetHistoricalTournamentParams
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public Gender? Gender { get; set; }              // Gender Filter
        public string? SearchName { get; set; }          // Name or Rank Search

        public DateTime? StartDateFrom { get; set; }     // Date filter from
        public DateTime? StartDateTo { get; set; }       // Date filter to


        public int? RankFrom { get; set; }               // Rank lower bound
        public int? RankTo { get; set; }                 // Rank upper bound
    }
}
