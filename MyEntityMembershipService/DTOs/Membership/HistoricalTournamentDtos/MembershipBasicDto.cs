using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.HistoricalTournamentDtos
{
    public class MembershipBasicDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string ImageUrl { get; set; }
        public string UserId { get; set; }

    }
}
