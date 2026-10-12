namespace MyEntityMembershipService.DTOs.Membership.HistoricalTournamentDtos
{
    public class GetAllHistoricalTournamentResultsDto
    {
        public int Id { get; set; }
        public string TournamentName { get; set; }
        public string AthleteFullName { get; set; }
        public DateTime Date { get; set; }
        public string Race { get; set; }
        public TimeSpan Time { get; set; }
        public int Rank { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
