

namespace MyEntityMembershipService.DTOs.Entity.TeamsDtos
{
    public class AddNewTeamDto
    {
        public string TeamName { get; set; }
        public IFormFile TeamImage { get; set; }
        public int Gender { get; set; }
        public int SportID { get; set; }
        public string Description { get; set; }
        public string? CoachID { get; set; }
        public List<string> TeamMemberIDs { get; set; }

    }

}
