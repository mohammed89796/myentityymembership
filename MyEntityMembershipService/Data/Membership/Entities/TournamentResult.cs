namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class TournamentResult:BaseEntity
    {
        public int HistoricalTournamentId { get; set; }
        public HistoricalTournament HistoricalTournament { get; set; }

        public string? SwimmerNationalId { get; set; }
        public string? SwimmerPhoneNumber { get; set; }
        public string? NameAr { get; set; }   

        public TimeSpan? Time { get; set; }
        public int? RaceId { get; set; }
        public string? RaceName { get; set; }
        public DateOnly? RaceDate { get; set; }
        public string? Lane { get; set; }
        public int? Ranking { get; set; }

        // Link to membership
        public int? MembershipId { get; set; }
        public Membership? Membership { get; set; }
    }

}
