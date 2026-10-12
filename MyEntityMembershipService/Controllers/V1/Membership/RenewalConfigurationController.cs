using AutoMapper;
using MyEntityMembershipService.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using MyEntityMembershipService.DTOs.Membership.RenewalConfigurationDtos;
using MyEntityMembershipService.Services.Membership.RenewalConfigurationService;
using Microsoft.AspNetCore.Authorization;

namespace MyEntityMembershipService.Controllers.V1.Membership
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]

    public class RenewalConfigurationController : ControllerBase
    {
        private readonly IRenewalConfigurationService renewalConfigurationService;

        public RenewalConfigurationController(IRenewalConfigurationService renewalConfigurationService)
        {
            this.renewalConfigurationService = renewalConfigurationService;
        }

        // Get all Renewal Configurations
        [HttpGet("GetAllRenewalConfigurations")]
        public async Task<IResult> GetAllRenewalConfigurations()
        {
            var result = await renewalConfigurationService.GetAllConfigurations();
            return result.GetResponse();
        }

        // Get Renewal Configuration by ID
        [HttpGet("GetRenewalConfigurationById/{id}")]
        public async Task<IResult> GetRenewalConfigurationById(int id)
        {
            var result = await renewalConfigurationService.GetConfigurationById(id);
            return result.GetResponse();
        }

        // Get Renewal Configuration by Package ID
        [HttpGet("GetRenewalConfigurationByPackageId/{packageId}")]
        public async Task<IResult> GetRenewalConfigurationByPackageId(int packageId)
        {
            var result = await renewalConfigurationService.GetConfigurationByPackageId(packageId);
            return result.GetResponse();
        }

        // Create or Sync Renewal Configuration
        [HttpPost("SyncRenewalConfiguration")]
        public async Task<IResult> SyncRenewalConfiguration(int packageId, RenewalConfigurationDto dto)
        {
            var result = await renewalConfigurationService.SyncConfiguration(packageId, dto);
            return result.GetResponse();
        }

        // Update Renewal Configuration
        [HttpPut("UpdateRenewalConfiguration")]
        public async Task<IResult> UpdateRenewalConfiguration(int id, RenewalConfigurationDto dto)
        {
            var result = await renewalConfigurationService.UpdateConfiguration(id, dto);
            return result.GetResponse();
        }

        // Delete Renewal Configuration
        [HttpDelete("DeleteRenewalConfiguration/{id}")]
        public async Task<IResult> DeleteRenewalConfiguration(int id)
        {
            var result = await renewalConfigurationService.DeleteConfiguration(id);
            return result.GetResponse();
        }
    }
}
