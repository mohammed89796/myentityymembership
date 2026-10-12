using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Membership.FreezeConfigurtionDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Membership.FreezeConfigurationService;

namespace MyEntityMembershipService.Controllers.V1.Membership
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]

    public class FreezeConfigurationController : ControllerBase
    {
        private readonly IFreezeConfigurationService _service;

        public FreezeConfigurationController(IFreezeConfigurationService service)
            => _service = service;

        //[HttpGet("GetAllFreezeConfigurations")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<IResult> GetAll()
        //{
        //    var result = await _service.GetAllAsync();
        //    return result.GetResponse();
        //}

        //[HttpGet("GetFreezeConfigurationById/{id:int}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<IResult> GetById(int id)
        //{
        //    var result = await _service.GetByIdAsync(id);
        //    return result.GetResponse();
        //}

        [HttpGet("GetFreezeConfigurationByPackageId/{packageId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IResult> GetByPackageId(int packageId)
        {
            var result =await _service.GetByPackageId(packageId);
            return result.GetResponse();
        }

        //[HttpPost("AddFreezeConfiguration")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<IResult> Add(FreezeConfigurationDto dto)
        //{
        //    var result = await _service.AddAsync(dto);
        //    return result.GetResponse();
        //}

        //[HttpPut("UpdateFreezeConfiguration")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<IResult> Update(FreezeConfigurationDto dto)
        //{
        //    var result = await _service.UpdateAsync(dto);
        //    return result.GetResponse();
        //}

        [HttpPost("SyncFreezeConfiguration")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IResult> Sync(FreezeConfigurationSyncDto dto)
        {
            var result = await _service.SyncFreezeConfiguration(dto);
            return result.GetResponse();
        }

        [HttpDelete("DeleteFreezeConfiguration")]
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
