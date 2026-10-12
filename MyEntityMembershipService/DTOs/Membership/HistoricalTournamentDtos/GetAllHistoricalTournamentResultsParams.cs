using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.HistoricalTournamentDtos
{
    public class GetAllHistoricalTournamentResultsParams
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public List<int>? Races { get; set; }
        public Gender? Gender { get; set; }
        public string? SearchTerm { get; set; }   // Athlete name, tournament name, lane

        public int? RankFrom { get; set; }       
        public int? RankTo { get; set; }         

        public DateTime? DateFrom { get; set; }   
        public DateTime? DateTo { get; set; }     
    }
}
