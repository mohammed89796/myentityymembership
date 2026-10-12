namespace MyEntityMembershipService.DTOs.Entity.VoteDtos
{
    public class VoteDto
    {
        public int PollId { get; set; }
        public string UserId { get; set; }
        public List<int> ChoicesIds { get; set; } = new List<int>();
    }
}
