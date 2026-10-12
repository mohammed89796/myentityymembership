using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Entity.FederationDtos;
using MyEntityMembershipService.DTOs.Entity.TenantDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Entity.FederationService;

namespace MyEntityMembershipService.Controllers.V1.Entity.Federation
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]

    public class FederationController : ControllerBase
    {
        private readonly IFederationService federationService;

        public FederationController(IFederationService federationService)
        {
            this.federationService = federationService;
        }
        [HttpPost("CreateFederation")]
        public async Task<IResult> CreateFederationEntity([FromForm] AddFederationEntityDto dto)
        {
            var result = await federationService.CreateFederationEntity(dto);
            return result.GetResponse();
        }
        [HttpPost("Create-Federation")]
        public async Task<IResult> CreateFederation([FromForm] AddFederationDto dto)
        {
            var result = await federationService.CreateFederation(dto);
            return result.GetResponse();
        }
        [HttpGet("GetAll")]
        public async Task<IResult> GetAll([FromQuery] int tenentId, int? pageNumber = null, [FromQuery] int? pageSize = null)
        {
            int.TryParse(User.GetTenantId(), out var tenantId);

            var result = await federationService.GetFederationEntitiesAsync(tenantId > 0 ? tenantId : tenentId, pageNumber, pageSize);
            return result.GetResponse();
        }
        [HttpGet("GetFederationEntityById")]
        public async Task<IResult> GetFederationEntityById([FromQuery] int tenentId)
        {
            var result = await federationService.GetFederationEntityByIdAsync(tenentId);
            return result.GetResponse();
        }

        [HttpPut("Edit")]
        public async Task<IResult> Edit([FromForm] UpdateFederationDto dto)
        {
            var result = await federationService.EditFederationEntity(dto);
            return result.GetResponse();
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IResult> Delete(int id)
        {
            var result = await federationService.DeleteFederationEntity(id);
            return result.GetResponse();
        }
      
    }
}
