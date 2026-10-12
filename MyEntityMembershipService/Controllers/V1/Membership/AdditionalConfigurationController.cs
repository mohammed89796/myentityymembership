using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Membership.AdditionalConfigurationDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Membership.AdditionalConfigurationServ;

namespace MyEntityMembershipService.Controllers.V1.Membership
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]

    public class AdditionalConfigurationController : ControllerBase
    {
        private readonly IAdditionalConfigurationService additionalConfigurationService;

        public AdditionalConfigurationController(IAdditionalConfigurationService additionalConfigurationService)
        {
            this.additionalConfigurationService = additionalConfigurationService;
        }

        // Get all Additional Configurations
        [HttpGet("GetAllAdditionalConfigurations")]
        public async Task<IResult> GetAllAdditionalConfigurations()
        {
            var result = await additionalConfigurationService.GetAllAdditionalConfigurations();
            return result.GetResponse();
        }

        // Get Additional Configuration by ID
        [HttpGet("GetAdditionalConfigurationById/{id}")]
        public async Task<IResult> GetAdditionalConfigurationById(int id)
        {
            var result = await additionalConfigurationService.GetAdditionalConfigurationById(id);
            return result.GetResponse();
        }

        [HttpGet("GetAdditionalConfigurationByPackageId/{packageId}")]
        public async Task<IResult> GetAdditionalConfigurationByPackageId(int packageId)
        {
            var result = await additionalConfigurationService.GetAdditionalConfigurationByPackageId(packageId);
            return result.GetResponse();
        }

        // Create or Sync Additional Configuration
        [HttpPost("SyncAdditionalConfiguration")]  
        public async Task<IResult> SyncAdditionalConfiguration(int packageId, AdditionalConfigurationDto dto)
        {
            var result = await additionalConfigurationService.SyncAdditionalConfiguration(packageId, dto);
            return result.GetResponse();
        }

        // Update Additional Configuration
        [HttpPut("UpdateAdditionalConfiguration")]
        public async Task<IResult> UpdateAdditionalConfiguration(AdditionalConfigurationDto dto)
        {
            var result = await additionalConfigurationService.UpdateAdditionalConfiguration(dto.PackageId, dto);
            return result.GetResponse();
        }

        // Delete Additional Configuration
        [HttpDelete("DeleteAdditionalConfiguration/{id}")]
        public async Task<IResult> DeleteAdditionalConfiguration(int id)
        {
            var result = await additionalConfigurationService.DeleteAdditionalConfiguration(id);
            return result.GetResponse();
        }
    }
}
