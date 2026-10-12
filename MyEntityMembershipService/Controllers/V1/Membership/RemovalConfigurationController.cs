using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using MyEntityMembershipService.DTOs.Membership.RemovalConfigurationDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Membership.RemovalConfigurationService;

namespace MyEntityMembershipService.Controllers.V1.Membership
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]

    public class RemovalConfigurationController : ControllerBase
    {
        private readonly IRemovalConfigurationService _service;

        public RemovalConfigurationController(IRemovalConfigurationService service)
            => _service = service;

        [HttpGet("GetRemovalConfigurationByPackageId/{packageId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IResult> GetByPackageId(int packageId)
        {
            var result = await _service.GetByPackageId(packageId);
            return result.GetResponse();
        }


        [HttpPost("SyncRemovalConfiguration")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IResult> Sync(RemovalConfigurationSyncDto dto)
        {
            var result = await _service.SyncRemovalConfiguration(dto);
            return result.GetResponse();
        }

        [HttpDelete("DeleteRemovalConfiguration")]
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
