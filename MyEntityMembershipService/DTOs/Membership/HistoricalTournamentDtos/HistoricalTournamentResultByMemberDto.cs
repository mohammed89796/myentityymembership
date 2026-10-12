namespace MyEntityMembershipService.DTOs.Membership.HistoricalTournamentDtos
{
    public class HistoricalTournamentResultByMemberDto
    {
        public int Id { get; set; }
        public string Race { get; set; }
        public string Tournament { get; set; }
        public int TournamentId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public int Rank { get; set; }
        public string Lane { get; set; }
    }

}
