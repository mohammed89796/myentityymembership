using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.HistoricalTournamentDtos
{
    public class HistoricalTournamentUpdateDto
    {
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string OrganizerName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Gender Gender { get; set; }
        public int ClubRank { get; set; }
        public int SportId { get; set; }

    }

}
