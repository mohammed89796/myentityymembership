

namespace MyEntityMembershipService.DTOs.Entity.TeamsDtos
{
    public class TeamInfoDto
    {
        public string TeamImage { get; set; }
        public string TeamName { get; set; }
        public string CoachName { get; set; }
        public string Gender { get; set; }
        public string Description { get; set; }
    }

    public class PaginatedTeamMemberInfoDto
    {
        public int TotalPages { get; set; }
        public List<TeamMemberInfoDto> TeamMembers { get; set; }
    }
    public class TeamMemberInfoDto
    {
        public string MemberID { get; set; }
        public string UserImage { get; set; }
        public string Name { get; set; }
    }

}
