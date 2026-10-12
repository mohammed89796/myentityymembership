using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Entity.TermsOfServiceDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Entity.TermsOfServiceService;

namespace MyEntityMembershipService.Controllers.V1.Entity;


[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize] 
public class TermsOfServiceController : ControllerBase
{
    private readonly ITermsOfServiceService _service;

    public TermsOfServiceController(ITermsOfServiceService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IResult> Create([FromForm] TermsOfServiceCreateDto dto)
    {
        int.TryParse(User.GetTenantId(), out var tenantId);
        dto.TenantId = tenantId;

        var result = await _service.CreateTermsOfService(dto);
        return result.GetResponse();
    }

    [HttpPut]
    public async Task<IResult> Update([FromForm] TermsOfServiceUpdateDto dto)
    {
        int.TryParse(User.GetTenantId(), out var tenantId);
        dto.TenantId = tenantId;

        var result = await _service.UpdateTermsOfService(dto);
        return result.GetResponse();
    }

    [HttpDelete("{id:int}")]
    public async Task<IResult> Delete([FromRoute] int id)
    {
        int.TryParse(User.GetTenantId(), out var tenantId);

        var result = await _service.DeleteTermsOfService(id, tenantId);
        return result.GetResponse();
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<IResult> Get([FromRoute] int id)
    {
        var result = await _service.GetTermsOfService(id);
        return result.GetResponse();
    }

    [HttpGet]
    public async Task<IResult> GetAll([FromQuery] TermsOfServiceQueryParameters query)
    {
        int.TryParse(User.GetTenantId(), out var tenantId);
        if (tenantId > 0) query.TenantId = tenantId;

        var result = await _service.GetAllTermsOfService(query);
        return result.GetResponse();
    }
}