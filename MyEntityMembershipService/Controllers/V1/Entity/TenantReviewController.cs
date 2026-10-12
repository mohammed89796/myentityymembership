using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Entity.TenantReviewDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Entity.TenantReviewService;

namespace MyEntityMembershipService.Controllers.V1.Entity;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class TenantReviewController : ControllerBase
{
    private readonly ITenantReviewService _service;

    public TenantReviewController(ITenantReviewService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IResult> Create([FromBody] TenantReviewCreateDto dto)
    {
        
        dto.UserId = User.GetUserId() ?? dto.UserId ?? string.Empty;

        var result = await _service.CreateTenantReview(dto);
        return result.GetResponse();
    }

    

    [HttpDelete("/user/{userId}")]
    public async Task<IResult> Delete([FromRoute]  string userId)
    {
        int.TryParse(User.GetTenantId(), out var tenantId);

        var result = await _service.DeleteTenantReview(userId, tenantId);
        return result.GetResponse();
    }
    
    [HttpGet]
    public async Task<IResult> GetAll([FromQuery] TenantReviewQueryParameters query)
    {

        var result = await _service.GetAllTenantReviews(query);
        return result.GetResponse();
    }
    [HttpGet("{tenantId:int}/reviews/summary")]
    public async Task<IResult> GetReviewSummary([FromRoute] int tenantId)
    {
        var result = await _service.GetTenantReviewSummary(tenantId);
        return result.GetResponse();
    }
}
