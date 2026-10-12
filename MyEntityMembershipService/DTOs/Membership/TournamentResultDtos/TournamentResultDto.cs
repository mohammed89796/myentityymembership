namespace MyEntityMembershipService.DTOs.Membership.TournamentResultDtos
{
    public class TournamentResultDto
    {
        public int Id { get; set; }
        public int MembershipId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public string Time { get; set; }
        public string RaceName { get; set; }
        public int Ranking { get; set; }
    }

}
