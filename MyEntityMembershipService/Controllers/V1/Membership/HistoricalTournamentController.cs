using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Membership.HistoricalTournamentDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Membership.HistoricalTournamentService;

namespace MyEntityMembershipService.Controllers.V1.Membership
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    public class HistoricalTournamentController : ControllerBase
    {
        private readonly IHistoricalTournamentService _service;

        public HistoricalTournamentController(IHistoricalTournamentService service)
        {
            _service = service;
        }

        [HttpPost("Create")]
        public async Task<IResult> Create([FromBody] CreateHistoricalTournamentDto dto)
        {
            var result = await _service.AddAsync(dto);
            return result.GetResponse();
        }

        [HttpGet]
        public async Task<IResult> GetAll([FromQuery] GetHistoricalTournamentParams parameters)
        {
            var result = await _service.GetAllAsync(parameters);
            return result.GetResponse();
        }
        [HttpGet("GetAllWithoutPagination")]
        public async Task<IResult> GetAllWithoutPagination()
        {
            var result = await _service.GetAllWithoutPaginationAsync();
            return result.GetResponse();
        }

        [HttpGet("{id}")]
        public async Task<IResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result.GetResponse();
        }

        [HttpPut("{id}")]
        public async Task<IResult> Update(int id, [FromBody] HistoricalTournamentUpdateDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return result.GetResponse();
        }

        [HttpGet("TournamentResults/{id}")]
        public async Task<IResult> GetTournamentResults(int id,[FromQuery] GetAllTournamentResultsParams parameters)
        {
            var result = await _service.GetTournamentResultsAsync(id, parameters);
            return result.GetResponse();
        }
        [HttpDelete("TournamentResult/{id}")]
        public async Task<IResult> DeleteTournamentResult(int id)
        {
            var result = await _service.DeleteTournamentResult(id);
            return result.GetResponse();
        }
        [HttpGet("GetAllHistoricalTournamentResults")]
        public async Task<IResult> GetTournamentResults([FromQuery] GetAllHistoricalTournamentResultsParams parameters)
        {
            var result = await _service.GetAllHistoricalTournamentResultsAsync( parameters);
            return result.GetResponse();
        }
        [HttpGet("GetHistoricalResultsByMembership")]
        public async Task<IResult> GetTournamentResults([FromQuery] HistoricalTournamentResultByMemberParams parameters)
        {
            var result = await _service.GetHistoricalResultsByMembershipAsync(parameters);
            return result.GetResponse();
        }
        [HttpGet("GetHistoricalResultsGraph")]
        public async Task<IResult> GetHistoricalResultsGraph([FromQuery] HistoricalTournamentGraphParams parameters)
        {
            var result = await _service.GetHistoricalResultsGraphAsync(parameters);
            return result.GetResponse();
        }
        [HttpPost("AddResult")]
        public async Task<IResult> AddResult([FromBody] AddHistoricalTournamentResultDto dto)
        {
            var result = await _service.AddHistoricalTournamentResultAsync(dto);
            return result.GetResponse();
        }

        [HttpGet("GetResultById/{id}")]
        public async Task<IResult> GetResultById(int id)
        {
            var result = await _service.GetHistoricalTournamentResultByIdAsync(id);
            return result.GetResponse();
        }

        [HttpDelete("{id}")]
        public async Task<IResult> SoftDelete(int id)
        {
            var result = await _service.SoftDeleteAsync(id);
            return result.GetResponse();
        }
    }
}
