namespace MyEntityMembershipService.DTOs.Membership.HistoricalTournamentDtos
{
    public class HistoricalTournamentGraphParams
    {
        public int MembershipId { get; set; }
        public int SportId { get; set; }
        public List<int>? TournamentIds { get; set; }
        public List<int>? RaceIds { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }

    public class HistoricalTournamentGraphSeriesDto
    {
        public int? RaceId { get; set; }
        public string Race { get; set; } = string.Empty;
        public List<HistoricalTournamentGraphPointDto> Points { get; set; } = [];
    }

    public class HistoricalTournamentGraphPointDto
    {
        public int ResultId { get; set; }
        public int TournamentId { get; set; }
        public string Tournament { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public double TimeInSeconds { get; set; }
        public int? Rank { get; set; }
        public string Lane { get; set; } = string.Empty;
    }
}
