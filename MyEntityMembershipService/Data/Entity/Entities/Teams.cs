using Amazon.S3.Model;
using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class Team : BaseEntity
    {
        public string TeamName { get; set; }
        public string TeamImage { get; set; }
        public int Gender { get; set; }
        public int SportID { get; set; }
        public Sports Sport { get; set; }   
        public string Description { get; set; }
        public string? CoachID { get; set; }
        public ICollection<TeamMembers>? TeamMembers { get; set; }

    }
}
