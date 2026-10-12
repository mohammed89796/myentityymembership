using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Entity.TeamsDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.TeamManagementService
{

    public interface ITeamManagementService
    {
        Task<Response<bool>> CreateTeam(AddNewTeamDto teamDto);

        Task<Response<PaginationToReturnDto<GetTeamsListDto>>> GetTeamsList(
            TeamsFilterDto? filters,
            int pageNumber,
            int pageSize
        );

        Task<Response<GetTeamByIdDto>> GetTeamByIdAsync(int teamId);

        Task<Response<List<TeamMemberInfoDto>>> GetTeamMembersByTeamIdAsync(int teamId);

        Task<Response<bool>> DeleteTeamAsync(int teamId);
    }
}
