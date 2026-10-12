using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Entity.TeamsDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Entity.TeamManagementService;
using MyEntityMembershipService.Dtos;
using System.Threading.Tasks;

namespace MyEntityMembershipService.Controllers.V1.Entity.Teams
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize("ClientIdPolicy")]
    public class TeamManagementController : ControllerBase
    {
        private readonly ITeamManagementService _teamService;

        public TeamManagementController(ITeamManagementService teamService)
        {
            _teamService = teamService;
        }

        [HttpPost("CreateNewTeam")]
        public async Task<IResult> CreateNewTeam([FromForm] AddNewTeamDto request)
            => (await _teamService.CreateTeam(request)).GetResponse();

        [HttpGet("GetTeamsList")]
        public async Task<IResult> GetTeamsList([FromQuery] TeamsFilterDto? filters, int pageNumber = 1, int pageSize = 10)
            => (await _teamService.GetTeamsList(filters, pageNumber, pageSize)).GetResponse();

        [HttpGet("GetTeamById/{teamId}")]
        public async Task<IResult> GetTeamById(int teamId)
            => (await _teamService.GetTeamByIdAsync(teamId)).GetResponse();

        [HttpGet("GetTeamMembers/{teamId}")]
        public async Task<IResult> GetTeamMembers(int teamId)
            => (await _teamService.GetTeamMembersByTeamIdAsync(teamId)).GetResponse();

        [HttpDelete("DeleteTeam/{teamId}")]
        public async Task<IResult> DeleteTeam(int teamId)
            => (await _teamService.DeleteTeamAsync(teamId)).GetResponse();
    }
}
