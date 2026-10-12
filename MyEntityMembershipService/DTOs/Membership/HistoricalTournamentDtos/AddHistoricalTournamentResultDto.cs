namespace MyEntityMembershipService.DTOs.Membership.HistoricalTournamentDtos
{
    public class AddHistoricalTournamentResultDto
    {
        public int MembershipId { get; set; }
        public int HistoricalTournamentId { get; set; }
        public int? RaceId { get; set; }
        public string? RaceName { get; set; } = null!;
        public TimeSpan Time { get; set; }
        public DateOnly Date { get; set; }
        public int Rank { get; set; }
        public string? Lane { get; set; } = null!;
        public RaceDto? Race { get; set;}
    }

    public class RaceDto
    {
        public int RaceId { get;set; }
    
        public string RaceName { get; set;}
    }

}
