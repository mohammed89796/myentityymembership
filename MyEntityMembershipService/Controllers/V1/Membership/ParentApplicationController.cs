using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Membership.ParentApplicationFormDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Membership.ParentApplicationFormService;

namespace MyEntityMembershipService.Controllers.V1.Membership
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    public class ParentApplicationController:ControllerBase
    {
        private readonly IParentApplicationFormService _parentApplicationFormService;
        public ParentApplicationController(IParentApplicationFormService parentApplicationFormService)
        {
            _parentApplicationFormService = parentApplicationFormService;
        }

        [HttpPost("")]
        public async Task<IResult> CreateParentApplicationForm([FromBody] CreateParentApplicationFormDto dto)
        {
            dto.TenantId = User.GetParsedTenantId();
            var result = await _parentApplicationFormService.CreateParentApplicationFormAsync(dto);
            return result.GetResponse();
        }


        [HttpGet("")]
        public async Task<IResult> GetParentApplicationForms([FromQuery] ParentApplicationFormFilterDto filterDto)
        {
            filterDto.TenantId = User.GetParsedTenantId();
            var result = await _parentApplicationFormService.GetParentApplicationFormsAsync(filterDto);
            return result.GetResponse();
        }

        [HttpGet("{id}")]
        public async Task<IResult> GetParentApplicationFormById(int id)
        {
            var tenantId = User.GetParsedTenantId();
            var result = await _parentApplicationFormService.GetParentApplicationFormByIdAsync(id, tenantId);
            return result.GetResponse();
        }


        [HttpPut("")]
        public async Task<IResult> UpdateParentApplicationForm([FromBody] UpdateParentApplicationFormDto dto)
        {
            dto.TenantId = User.GetParsedTenantId();
            var result = await _parentApplicationFormService.UpdateParentApplicationFormAsync(dto);
            return result.GetResponse();
        }


        [HttpDelete("{id}")]
        public async Task<IResult> DeleteParentApplicationForm(int id)
        {
            var tenantId = User.GetParsedTenantId();
            var result = await _parentApplicationFormService.DeleteParentApplicationFormAsync(id, tenantId);
            return result.GetResponse();
        }


    }
}
