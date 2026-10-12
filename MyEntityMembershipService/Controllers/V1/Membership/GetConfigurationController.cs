using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using MyEntityMembershipService.Extensions;

using MyEntityMembershipService.Services.Membership.GetConfigurationService;

namespace MyEntityMembershipService.Controllers.V1.Membership
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]

    public class GetConfigurationController : ControllerBase
    {
        private readonly IGetConfigurationService _service;

        public GetConfigurationController(IGetConfigurationService service)
            => _service = service;

        [HttpGet("GetConfigurationByPackageId/{packageId:int}/{requestTypesId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IResult> GetConfigurationByPackageId(int packageId, int requestTypesId, int? subMemberId = null)
        {
            var result = await _service.GetConfiguartionAsync(packageId, requestTypesId, subMemberId);
            return result.GetResponse();
        }
    }
}
