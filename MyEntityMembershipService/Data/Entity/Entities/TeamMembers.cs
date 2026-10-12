using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class TeamMembers : BaseEntity
    {
        public int TeamID { get; set; }
        public string MemberID { get; set; }

    }
}
