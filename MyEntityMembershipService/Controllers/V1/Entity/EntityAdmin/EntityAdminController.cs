using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Entity.EntityAdminDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Services.Entity.EntityAdminService;

namespace MyEntityMembershipService.Controllers.V1.Entity.EntityAdmin
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    public class EntityAdminController : ControllerBase
    {
        private readonly IEntityAdminService entityAdminService;

        public EntityAdminController(IEntityAdminService entityAdminService)
        {
            this.entityAdminService = entityAdminService;
        }

        [HttpPost("AddEntityAdmin")]
        public async Task<IResult> AddEntityAdmin(AddEntityAdminDto entityAdminDto)
        {
            int FederationId = int.Parse(User.FindFirst("tenantId")?.Value);
            if (!entityAdminDto.TenantId.HasValue)
                entityAdminDto.TenantId = FederationId; 
            var result = await entityAdminService.AddEntityAdmin(entityAdminDto);
            return result.GetResponse();
        }
        [HttpPost("Set-entity-admin")]
        public async Task<IResult> SetEntityAdmin(AddEntityAdminDto entityAdminDto)
        {
            
            var result = await entityAdminService.AddEntityAdmin(entityAdminDto);
            return result.GetResponse();
        }

        [HttpPut("UpdateEntityAdmin")]
        public async Task<IResult> UpdateEntityAdmin(UpdateEntityAdminDto entityAdminDto)
        {
            var result = await entityAdminService.UpdateEntityAdmin(entityAdminDto);
            return result.GetResponse();
        }

        [HttpPut("DeactiveEntityAdmin/{userId}")]
        public async Task<IResult> DeactiveEntityAdmin(string userId)
        {
            var result = await entityAdminService.DeactiveEntityAdmin(userId);
            return result.GetResponse();
        }

        [HttpGet("GetEntityAdmins")]
        public async Task<IResult> GetEntityAdmins([FromQuery]GetEntityAdminsParametersDto parameters)
        {
            int FederationId = int.Parse(User.FindFirst("tenantId")?.Value);
            if(! parameters.TenantId.HasValue)
                parameters.TenantId = FederationId;
            var result = await entityAdminService.GetEntityAdmins(parameters);
            return result.GetResponse();
        }

        [HttpGet("GetTenantCoachAdmins")]
        public async Task<IResult> GetTenantCoachAdmins([FromQuery] GetTenantCoachAdminsParametersDto parameters)
        {
            if (!int.TryParse(User.GetTenantId(), out var tenantId))
                return Results.BadRequest("Invalid or missing tenant ID.");

            var result = await entityAdminService.GetTenantCoachAdmins(tenantId, parameters);
            return result.GetResponse();
        }

        [HttpGet("GetFederationEntitiesAdmins")]
        public async Task<IResult> GetFederationEntitiesAdmins([FromQuery]GetFederationEntitiesAdminsParametersDto parameters)
        {
            var tenantIdValue = User.GetTenantId();

            if (!int.TryParse(tenantIdValue, out int federationId))
                return Results.BadRequest("Invalid or missing tenant ID.");
            parameters.FederationId = federationId;

            var result = await entityAdminService.GetFederationEntitiesAdmins(parameters);
            return result.GetResponse();
        }

        [HttpGet("GetTenantAdmin/{userId}")]
        public async Task<IResult> GetTenantAdminByUserId(string userId)
        {
            var result = await entityAdminService.GetTenantAdminByUserId(userId);
            return result.GetResponse();
        }


        [HttpPut("UpdateEntityAdminStatus")]
        public async Task<IResult> UpdateEntityAdminStatus()
        {
            string UserId = User.GetUserId();
            var result = await entityAdminService.UpdateEntityAdminStatus(UserId);
            return result.GetResponse();
        }
    }
}
