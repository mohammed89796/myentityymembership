using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Entity.CoreUtilities;

namespace MyEntityMembershipService.Controllers.V1.Entity.Venue
{

    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize("ClientIdPolicy")]
    public class CoreUtilitiesController : ControllerBase
    {
        private readonly ICoreUtilitiesService _coreUtilitieService;

        public CoreUtilitiesController(ICoreUtilitiesService coreUtilitieService)
        {
            _coreUtilitieService = coreUtilitieService;
        }

        [HttpGet("CreateCoreUtilitie")]
        public async Task<IResult> CreateCoreUtilitie(string typename)

            => (await _coreUtilitieService.CreateCoreUtilitie(typename)).GetResponse();

        [HttpGet("GetCoreUtilitiesDropDown")]
        public async Task<IResult> GetCoreUtilitiesDropDown()

            => (await _coreUtilitieService.GetCoreUtilitiesDropDown()).GetResponse();

    }
}
