using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Membership.PackageRequestAuthorizationDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Membership.ConfigrationFieldResponseService;

namespace MyEntityMembershipService.Controllers.V1.Membership
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]

    public class ConfigrationFieldResponseController : ControllerBase
    {
        private readonly IConfigrationFieldResponseService _configrationFieldResponseService;

        public ConfigrationFieldResponseController(IConfigrationFieldResponseService configrationFieldResponseService)
        {
            _configrationFieldResponseService = configrationFieldResponseService;
        }

        [HttpPost("AddConfigrationFieldResponse")]
        public async Task<IResult> AddConfigrationFieldResponse([FromBody] List<ConfigrationFieldResponseDto> dtos)
        {
            var result = await _configrationFieldResponseService.AddConfigrationFieldResponses(dtos);
            return result.GetResponse();
        }

        [HttpPut("UpdateConfigrationFieldResponse")]
        public async Task<IResult> UpdateConfigrationFieldResponse([FromBody] ConfigrationFieldResponseDto dto)
        {
            var result = await _configrationFieldResponseService.UpdateConfigrationFieldResponse(dto);
            return result.GetResponse();
        }

        [HttpDelete("DeleteConfigrationFieldResponse/{id}")]
        public async Task<IResult> DeleteConfigrationFieldResponse(int id)
        {
            var result = await _configrationFieldResponseService.DeleteConfigrationFieldResponse(id);
            return result.GetResponse();
        }

        [HttpGet("GetAllConfigrationFieldResponses")]
        public async Task<IResult> GetAllConfigrationFieldResponses()
        {
            var result = await _configrationFieldResponseService.GetAllConfigrationFieldResponses();
            return result.GetResponse();
        }

        [HttpGet("GetConfigrationFieldResponseById/{id}")]
        public async Task<IResult> GetConfigrationFieldResponseById(int id)
        {
            var result = await _configrationFieldResponseService.GetConfigrationFieldResponseById(id);
            return result.GetResponse();
        }
        [HttpPut("sync/{packageId}")]
        public async Task<IResult> SyncConfigrationFieldResponses(int packageId, [FromBody] List<ConfigrationFieldResponseDto> newForms)
        {          
            var result = await _configrationFieldResponseService.SyncConfigrationFieldResponses(packageId, newForms);
            return result.GetResponse(); // Assuming `GetResponse()` is an extension method for formatting the response
        }


        [HttpGet("GetConfigurationFieldResponses")]
        public async Task<IResult> GetConfigurationFieldResponses(int requestTypeId,int packageID)
        {          
           var result = await _configrationFieldResponseService.GetConfigrationFieldResponse(requestTypeId, packageID);
           return result.GetResponse();
        }
    }
}
