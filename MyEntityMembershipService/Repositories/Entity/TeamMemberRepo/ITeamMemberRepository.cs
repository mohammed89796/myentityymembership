


using MyEntityMembershipService.Repositories.GenericRepo;
using System.Threading.Tasks;

namespace MyEntityMembershipService.Repositories.Entity.TeamMemberRepo
{
    public interface ITeamMemberRepository : IGenericRepository<TeamMembers>
    {
        Task<List<TeamMembers>> GetTeamMembersByTeamID(int teamID);
    }
}
