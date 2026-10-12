

namespace MyEntityMembershipService.DTOs.Entity.TeamsDtos
{
    public class GetTeamsListDto
    {
        public int TeamID { get; set; }
        public string TeamName { get; set; }
        public string ImageURL { get; set; }
        public int SportID { get; set; }
        public string SportName { get; set; }
        public string Gender { get; set; }
        public string CoatchName { get; set; } 
        public int? NoOfMembers { get; set; }


    }

}
