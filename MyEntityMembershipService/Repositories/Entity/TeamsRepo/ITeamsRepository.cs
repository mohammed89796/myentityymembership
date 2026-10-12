


using MyEntityMembershipService.DTOs.Entity.TeamsDtos;
using MyEntityMembershipService.Repositories.GenericRepo;
using System.Threading.Tasks;

namespace MyEntityMembershipService.Repositories.Entity.TeamsRepo
{
    public interface ITeamsRepository : IGenericRepository<Team>
    {
        Task<List<GetTeamsListDto>> GetTeamsListAsync(TeamsFilterDto? filters);
        Task<TeamInfoDto> GetTeamDetailsAsync(int teamId);
        Task<PaginatedTeamMemberInfoDto> GetTeamMembersAsync(int teamId, int pageNumber);
        Task<GetEditTeamDto> GetEditTeamDetails(int teamId);
    }
}
