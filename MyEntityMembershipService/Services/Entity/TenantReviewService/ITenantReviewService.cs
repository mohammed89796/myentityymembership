using MyEntityMembershipService.DTOs.Entity.TenantReviewDtos;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Dtos;

namespace MyEntityMembershipService.Services.Entity.TenantReviewService;

public interface ITenantReviewService
{
    Task<Response<string>> CreateTenantReview(TenantReviewCreateDto dto);
    Task<Response<string>> DeleteTenantReview(string userId, int tenantId);
    Task<Response<PaginationToReturnDto<TenantReviewReadDto>>> GetAllTenantReviews(TenantReviewQueryParameters queryParameters);
    
    Task<Response<TenantReviewSummaryDto>> GetTenantReviewSummary(int tenantId);
}
