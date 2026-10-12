using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Membership.AreaDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Membership.AreaServ;
using System.Security.Claims;

namespace MyEntityMembershipService.Controllers.V1.Membership
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]

    public class AreaController : ControllerBase
    {
        private readonly IAreaService areaService;

        public AreaController( IAreaService areaService)
        {
            this.areaService = areaService;
        }

        [HttpPost("AddArea")]
        public async Task<IResult> AddArea(AddAreaDto areaDto)
        {
            if (!areaDto.FederationId.HasValue)
                areaDto.FederationId = int.Parse(User.FindFirstValue("tenantId"));
            var result = await areaService.AddArea(areaDto);
            return result.GetResponse();
        }

        [HttpGet("GetAllAreas")]
        public async Task<IResult> GetAllAreas(int PageIndex , int PageSize , string? SearchBy)
        {
            int.TryParse(User.GetTenantId(), out int FederationId);
            //var FederationId = int.Parse(User.FindFirstValue("tenantId"));
            var result = await areaService.GetAllAreas(FederationId, SearchBy, PageIndex, PageSize);
            return result.GetResponse();
        }

        [HttpGet("GetAreaWithBankAccounts")]
        public async Task<IResult> GetAreaWithBankAccounts(int AreaId)
        {
            var result = await areaService.GetAreaWithBankAccounts(AreaId);
            return result.GetResponse();
        }

        [HttpPut("UpdateArea")]
        public async Task<IResult> UpdateArea(AreaWithAccountsDto areaDto)
        {
            var result = await areaService.UpdateArea(areaDto);
            return result.GetResponse();
        }

        [HttpGet("GetAllAreaPricing")]
        public async Task<IResult> GetAllAreaPricing([FromQuery]GetAllAreaPricingParametersDto parameters )
        {
            var result = await areaService.GetAllAreaPricing(parameters);
            return result.GetResponse();
        }

        [HttpPut("UpdateAreaPricing")]
        public async Task<IResult> UpdateAreaPricing(UpdateAreaPricingDto areaPricingDto)
        {
            var result = await areaService.UpdateAreaPricing(areaPricingDto);
            return result.GetResponse();
        }

        [HttpGet("GetAreaConfigurations")]
        public async Task<IResult> GetAreaConfigurations(int AreaId)
        {
            var result = await areaService.GetAreaConfigurations(AreaId);
            return result.GetResponse();
        }

        [HttpPost("AddAreaConfiguration")]
        public async Task<IResult> AddAreaConfiguration(AddAreaConfigurationDto areaConfigurationDto)
        {
            var result = await areaService.AddAreaConfiguration(areaConfigurationDto);
            return result.GetResponse();
        }

        [HttpPut("UpdateAreaConfiguration")]
        public async Task<IResult> UpdateAreaConfiguration(UpdateAreaConfigurationDto areaConfigurationDto)
        {
            var result = await areaService.UpdateAreaConfiguration(areaConfigurationDto);
            return result.GetResponse();
        }

        [HttpDelete("DeleteAreaConfiguration")]
        public async Task<IResult> DeleteAreaConfiguration(int Id)
        {
            var result = await areaService.DeleteAreaConfiguration(Id);
            return result.GetResponse();
        }

        [HttpPost("AddAreaBankAccount")]
        public async Task<IResult> AddAreaBankAccount(AddAreaBankAccountDto parameters)
        {
            var result = await areaService.AddAreaBankAccount(parameters);
            return result.GetResponse();
        }

        [HttpGet("GetAllFederationAreas")]
        public async Task<IResult> GetAllFederationAreas(int? FederationId)
        {
            if (!FederationId.HasValue)
                FederationId = int.Parse(User.FindFirst("tenantId")?.Value);
            string userId = User.GetUserId()!;
            var result = await areaService.GetAllFederationAreas(FederationId.Value, userId);
            return result.GetResponse();
        }

        [HttpGet("GetAllFederationAreasByEntityId")]
        public async Task<IResult> GetAllFederationAreasByEntityId()
        {
            var entityId = int.Parse(User.FindFirst("tenantId")?.Value);
            var result = await areaService.GetAllFederationAreasByEntityId(entityId);
            return result.GetResponse();
        }
        [HttpGet("GetEntitiesByAreaWithoutPagination")]
        public async Task<IResult> GetAllFederationAreasByEntityId(int areaId)
        {
            var result = await areaService.GetEntitiesByAreaWithoutPagination(areaId);
            return result.GetResponse();
        }



        [HttpGet("GetEntitiesByArea")]
        public async Task<IResult> GetEntitiesByArea([FromQuery] GetEntitiesByAreaParametersDto parameters)
        {
            if (!parameters.FederationId.HasValue)
                parameters.FederationId = int.Parse(User.FindFirstValue("tenantId"));
            var result = await areaService.GetEntitiesByArea(parameters);
            return result.GetResponse();
        }

    }
}
