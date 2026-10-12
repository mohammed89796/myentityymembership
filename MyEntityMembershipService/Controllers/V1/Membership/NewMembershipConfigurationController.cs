using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs;
using MyEntityMembershipService.DTOs.Membership.NewMembershipConfigurationDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Membership.NewMembershipConfigurationService;
using System.Threading.Tasks;

namespace MyEntityMembershipService.Controllers.V1.Membership
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]

    public class NewMembershipConfigurationController : ControllerBase
    {
        private readonly INewMembershipConfigurationService newMembershipConfigurationService;

        public NewMembershipConfigurationController(INewMembershipConfigurationService newMembershipConfigurationService)
        {
            this.newMembershipConfigurationService = newMembershipConfigurationService;
        }

        [HttpGet("GetAllConfigurations")]
        public async Task<IResult> GetAllConfigurations()
        {
            var result = await newMembershipConfigurationService.GetAllConfigurations();
            return result.GetResponse();
        }

        [HttpGet("GetConfigurationById/{id}")]
        public async Task<IResult> GetConfigurationById(int id)
        {
            var result = await newMembershipConfigurationService.GetConfigurationById(id);
            return result.GetResponse();
        }

        [HttpGet("GetConfigurationByPackageId/{packageId}")]
        public async Task<IResult> GetConfigurationByPackageId(int packageId)
        {
            var result = await newMembershipConfigurationService.GetConfigurationByPackageId(packageId);
            return result.GetResponse();
        }

        [HttpPost("CreateConfiguration")]
        public async Task<IResult> CreateConfiguration(NewMembershipConfigurationDto dto)
        {
            var result = await newMembershipConfigurationService.CreateConfiguration(dto);
            return result.GetResponse();
        }

        [HttpPut("UpdateConfiguration/{id}")]
        public async Task<IResult> UpdateConfiguration(int id, NewMembershipConfigurationDto dto)
        {
            var result = await newMembershipConfigurationService.UpdateConfiguration(id, dto);
            return result.GetResponse();
        }

        [HttpDelete("DeleteConfiguration/{id}")]
        public async Task<IResult> DeleteConfiguration(int id)
        {
            var result = await newMembershipConfigurationService.DeleteConfiguration(id);
            return result.GetResponse();
        }

        [HttpPost("SyncConfiguration/{packageId}")]
        public async Task<IResult> SyncConfiguration(int packageId, NewMembershipConfigurationDto dto)
        {
            var result = await newMembershipConfigurationService.SyncConfiguration(packageId, dto);
            return result.GetResponse();
        }
    }
}
