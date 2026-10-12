using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyEntityMembershipService.DTOs.Membership.EntityDtos;
using MyEntityMembershipService.DTOs.Membership.FederationEntitiesDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Membership.EntityService;

namespace MyEntityMembershipService.Controllers.V1.Membership
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]

    public class FederationEntityController : ControllerBase
    {
        private readonly IEntityService federationService;

        public FederationEntityController(IEntityService federationService)
        {
            this.federationService = federationService;
        }
        [HttpPost("createfederationeentity")]
        public async Task<IResult> CreateFederationEntity([FromBody] EntityDto dto)
        {
            var result = await federationService.CreateFederationEntity(dto);
            return result.GetResponse();
        }
        [HttpGet("getallentities")]
        public async Task<IResult> GetAll([FromQuery] GetFederationEntitiesParameterDto parameters )
        {
            int.TryParse(User.GetTenantId(), out var tenantId);

            parameters.userId = User.GetUserId();
            parameters.tenentId = tenantId > 0 ? tenantId : parameters.tenentId;
            var result = await federationService.GetFederationEntitiesAsync(parameters);
            return result.GetResponse();
        }
        [HttpGet("getfederationentityById")]
        public async Task<IResult> GetFederationEntityById([FromQuery] int tenentId)
        {
            var result = await federationService.GetFederationEntityByIdAsync(tenentId);
            return result.GetResponse();
        }
        [HttpGet("GetEntityCounts")]
        public async Task<IResult> GetEntityCounts()
        {
            int.TryParse(User.GetTenantId(), out var tenantId);
            var result = await federationService.GetEntityCountsAsync(tenantId);
            return result.GetResponse();
        }

        [HttpPut("editentity/{id}")]
        public async Task<IResult> Edit([FromForm] UpdateEntityDto dto)
        {
            var result = await federationService.EditFederationEntity(dto);
            return result.GetResponse();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IResult> Delete(int id)
        {
            var result = await federationService.DeleteFederationEntity(id);
            return result.GetResponse();
        }
    }
}
