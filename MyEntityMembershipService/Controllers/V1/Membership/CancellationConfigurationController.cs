using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Membership.CancellationConfigurationDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Membership.CancellationConfigurationService;

namespace MyEntityMembershipService.Controllers.V1.Membership
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]

    public class CancellationConfigurationController : ControllerBase
    {
        private readonly ICancellationConfigurationService _service;

        public CancellationConfigurationController(ICancellationConfigurationService service)
            =>  _service = service;

        [HttpGet("GetCancellationConfigurationByPackageId/{packageId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IResult> GetByPackageId(int packageId)
        {
            var result = await _service.GetByPackageIdAsync(packageId);
            return result.GetResponse();
        }


        [HttpPost("SyncCancellationConfiguration")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IResult> Sync(CancellationConfigurationSyncDto dto)
        {
            var result = await _service.Sync(dto);
            return result.GetResponse();
        }

        [HttpDelete("DeleteCancellationConfiguration")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result.GetResponse();
        }

    }
}
