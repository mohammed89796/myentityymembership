namespace MyEntityMembershipService.DTOs.Membership.HistoricalTournamentDtos
{
    public class GetHistoricalTournamentResultDto
    {
        public int Id { get; set; }
        public string AthleteFullName { get; set; } = null!;
        public string TournamentName { get; set; } = null!;
        public string RaceName { get; set; } = null!;
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public int Rank { get; set; }
        public string Lane { get; set; } = null!;
    }

}
