
using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Entity.TeamsDtos;


using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.Teams
{
    public interface ITeamsService
    {
        Task<Response<bool>> CreateNewTeam(AddNewTeamDto request);
        Task<Response<List<GetTeamsListDto>>> GetTeamsList(TeamsFilterDto filters);
        Task<Response<GetEditTeamDto>> GetEditTeamDetails(int teamID);
        Task<Response<bool>> EditTeam(EditTeamDto request);
        Task<Response<bool>> DeleteTeamAsync(int teamId);
        Task<Response<TeamInfoDto>> GetTeamBasicDetails(int teamID);
        Task<Response<PaginatedTeamMemberInfoDto>> GetTeamMembers(int teamID, int pageNumber);
    }
}
