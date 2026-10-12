using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Entity.TeamsDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Entity.Teams;

namespace MyEntityMembershipService.Controllers.V1.Entity.Teams
{

    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize("ClientIdPolicy")]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamsService _teamsService;

        public TeamsController(ITeamsService teamsService)
        {
            _teamsService = teamsService;
        }

        [HttpPost("CreateNewTeam")]
        public async Task<IResult> CreateNewTeam([FromForm] AddNewTeamDto request)

            => (await _teamsService.CreateNewTeam(request)).GetResponse();

        [HttpPost("GetTeamsList")]
        public async Task<IResult> GetTeamsList(TeamsFilterDto? filters)

            => (await _teamsService.GetTeamsList(filters)).GetResponse();

        [HttpGet("GetEditTeamDetails")]
        public async Task<IResult> GetEditTeamDetails(int teamID)

            => (await _teamsService.GetEditTeamDetails(teamID)).GetResponse();

        [HttpPut("EditTeam")]
        public async Task<IResult> EditTeam([FromForm] EditTeamDto request)

            => (await _teamsService.EditTeam(request)).GetResponse();

        [HttpDelete("DeleteTeamAsync/{teamId}")]
        public async Task<IResult> DeleteTeamAsync(int teamId)

            => (await _teamsService.DeleteTeamAsync(teamId)).GetResponse();

        [HttpGet("GetTeamBasicDetails/{teamID}")]
        public async Task<IResult> GetTeamBasicDetails(int teamID)

            => (await _teamsService.GetTeamBasicDetails(teamID)).GetResponse();
        [HttpGet("GetTeamMembers")]
        public async Task<IResult> GetTeamMembers(int teamID, int pageNumber)

            => (await _teamsService.GetTeamMembers(teamID, pageNumber)).GetResponse();

    }
}
