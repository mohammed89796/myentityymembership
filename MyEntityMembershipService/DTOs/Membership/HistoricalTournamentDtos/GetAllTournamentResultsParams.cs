using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.HistoricalTournamentDtos
{
    public class GetAllTournamentResultsParams
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public List<int>? Races { get; set; }          // Race Filter
        public Gender? Gender { get; set; }     // Gender Filter

        public string? SearchName { get; set; } // Athlete full name or Lane

        public int? RankFrom { get; set; }      // Minimum rank
        public int? RankTo { get; set; }        // Maximum rank
    }
}
