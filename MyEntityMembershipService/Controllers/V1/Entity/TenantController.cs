using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Entity.TenantDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Entity.TenantService;
using System.Security.Claims;

namespace MyEntityMembershipService.Controllers.V1.Entity
{

    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]

    public class TenantController : ControllerBase
    {
        private readonly ITenantService tenantService;

        public TenantController(ITenantService tenantService)
        {
            this.tenantService = tenantService;
        }
        [HttpPost("AddTenant")]
        public async Task<IResult> AddTenant(AddTenantDto tenantDto)
        {
            var result = await tenantService.AddTenant(tenantDto);
            return result.GetResponse();
        }
        [HttpGet("GetAllTenants")]
        public async Task<IResult> GetAllTenants(string? searchName)
        {
            var result = await tenantService.GetAllTenants(searchName);
            return result.GetResponse();
        }
        [HttpGet("GetAllFederations")]
        public async Task<IResult> GetAllFederations(string? searchName)
        {
            var result = await tenantService.GetAllFederationss(searchName);
            return result.GetResponse();
        }

        [HttpPost("GetTenantsByIds")]
        public async Task<IResult> GetTenantsByIds(List<int> tenantIds)

            => (await tenantService.GetTenantsByListOfIdsAsync(tenantIds)).GetResponse();
        [HttpPost("GetFedByIdAsync")]
        public async Task<IResult> GetFedByIdAsync(int fedId)

           => (await tenantService.GetFedByIdAsync(fedId)).GetResponse();



        [HttpGet("GetTenantById/{tenantId}")]

        public async Task<IResult> GetTenantById(int tenantId)
            => (await tenantService.GetTenantByIdAsync(tenantId)).GetResponse();

        [HttpPut("UpdateTenant")]
        public async Task<IResult> UpdateTenant([FromForm] UpdateTenantDto updateTenantDto)
        {
            var result = await tenantService.UpdateTenant(updateTenantDto);
            return result.GetResponse();
        }

        [HttpGet("CheckTanantTypeById")]
        public async Task<IResult> CheckTanantTypeById(int tanantId)
        {
            var result = await tenantService.CheckTanantTypeById(tanantId);
            return result.GetResponse();
        }

        [HttpGet("GetFederationEntities")]
        public async Task<IResult> GetFederationEntities()
        {
            int FederationId = int.Parse(User.FindFirst("tenantId")?.Value);
            var result = await tenantService.GetFederationEntities(FederationId);
            return result.GetResponse();
        }
        [HttpGet("GetTenantDetails")]
        public async Task<IResult> GetTenantDetails(int tanantId)
        {
            var result = await tenantService.GetTenantDetails(tanantId);
            return result.GetResponse();
        }

        [HttpGet("GetEntitiesByArea")]
        public async Task<IResult> GetEntitiesByArea([FromQuery] GetEntitiesByAreaParametersDto parameters)
        {
            if (!parameters.FederationId.HasValue)
                parameters.FederationId = int.Parse(User.FindFirstValue("tenantId"));
            var result = await tenantService.GetEntitiesByArea(parameters);
            return result.GetResponse();
        }

        [HttpGet("GetAreaStatistics")]
        public async Task<IResult> GetAreaStatistics(int AreaId)
        {
            var result = await tenantService.GetAreaStatistics(AreaId);
            return result.GetResponse();
        }

        [HttpGet("GetAllSuperEntities")]
        public async Task<IResult> GetAllSuperEntitiesAsync([FromQuery] SuperEntityDto dto)
        {
            var result = await tenantService.GetAllSuperEntitiesAsync(dto);
            return result.GetResponse();
        }

        [HttpGet("EntityType")]
        public async Task<IResult> GetEntityType(string? searchTerm)
        {
            var result = await tenantService.GetEntityTypeAsync(searchTerm);
            return result.GetResponse();
        }

        [HttpPost("ReachOutToTenant")]
        public async Task<IResult> ReachOutToTenantAsync([FromForm] ReachOutToTenant dto)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown User";

            var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? "Unknown Email";

            var result = await tenantService.ReachOutToTenantAsync(dto, userName, userEmail);

            return result.GetResponse();
        }
    }
}
