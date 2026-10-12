using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Membership.ParentApplicationConfigurationDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Membership.ParentApplicationService;

namespace MyEntityMembershipService.Controllers.V1.Membership
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    public class ParentApplicationConfigurationController : ControllerBase
    {
        private readonly IParentApplicationConfigurationService _parentApplicationConfigurationService;

        public ParentApplicationConfigurationController(IParentApplicationConfigurationService parentApplicationConfigurationService)
        {
            _parentApplicationConfigurationService = parentApplicationConfigurationService;
        }


        [HttpPost]
        public async Task<IResult> CreateParentApplicationConfig([FromBody] CreateParentApplicationConfigDto createDto)
        {
            createDto.TenantId = User.GetParsedTenantId();
            var result = await _parentApplicationConfigurationService.CreateParentApplicationConfigAsync(createDto);
            return result.GetResponse();
        }


        [HttpPut("{id}")]
        public async Task<IResult> UpdateParentApplicationConfig(int id,[FromBody] UpdateParentApplicationConfigDto updateDto)
        {
            updateDto.TenantId = User.GetParsedTenantId();
            var result = await _parentApplicationConfigurationService.UpdateParentApplicationConfigAsync(id, updateDto);
            return result.GetResponse();
        }



        [HttpGet("Package/{packageId}")]
        public async Task<IResult> GetParentApplicationConfigByPackageId(int packageId)
        {
            var result = await _parentApplicationConfigurationService.GetParentApplicationConfigsByPackageIdAsync(packageId);
            return result.GetResponse();
        }

    }
}
