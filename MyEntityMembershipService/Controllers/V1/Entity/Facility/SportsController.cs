using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Entity.SportsService;

namespace MyEntityMembershipService.Controllers.V1.Entity.Facility
{

    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize("ClientIdPolicy")]
    public class SportsController : ControllerBase
    {
        private readonly ISportsService _sportService;

        public SportsController(ISportsService sportService)
        {
            _sportService = sportService;
        }

        [HttpGet("CreateSport")]
        public async Task<IResult> CreateSport(string sportName)

            => (await _sportService.CreateSport(sportName)).GetResponse();

        [HttpGet("GetSportsDropDown")]
        public async Task<IResult> GetSportsDropDown()

            => (await _sportService.GetSportsDropDown()).GetResponse();

    }
}
