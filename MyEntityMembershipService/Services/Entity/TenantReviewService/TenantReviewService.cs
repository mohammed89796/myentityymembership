using System.Linq;
using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Entity.TenantReviewDtos;
using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.SpecificationPattern;
using Serilog;

namespace MyEntityMembershipService.Services.Entity.TenantReviewService;

public class TenantReviewService : ITenantReviewService
{
    private readonly IUnitOfWork _unitOfWork;

    public TenantReviewService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Response<string>> CreateTenantReview(TenantReviewCreateDto dto)
    {
        try
        {
            var specs = new GeneralSpecifications<TenantReview>(t => t.UserId == dto.UserId && t.TenantId == dto.TenantId);
            var existTenantReview = await _unitOfWork.Repository<TenantReview>().GetWithSpecifications(specs);
            if (existTenantReview != null)
            {
                existTenantReview.Comment = dto.Comment;
                existTenantReview.TenantId = dto.TenantId;
                existTenantReview.Rating = dto.Rating;
                existTenantReview.ImageUrl = dto.ImageUrl ?? existTenantReview.ImageUrl;
                existTenantReview.Name = dto.Name;
                _unitOfWork.Repository<TenantReview>().Update(existTenantReview);
                
            }
            else
            {
                var tenantReview = new TenantReview
                {
                    TenantId = dto.TenantId,
                    UserId = dto.UserId,
                    Name = dto.Name,
                    ImageUrl = dto.ImageUrl,
                    Rating = dto.Rating,
                    Comment = dto.Comment
                };
                await _unitOfWork.Repository<TenantReview>().Add(tenantReview);
            }
            await _unitOfWork.Save();

            return new ResponseHandler().Success("Tenant review created successfully");
        }
        catch (Exception e)
        {
            Log.Error(e, "Error creating tenant review: {Message}", e.Message);
            return new ResponseHandler().BadRequest<string>("Something went wrong");
        }
    }


    public async Task<Response<string>> DeleteTenantReview(string userId, int tenantId)
    {
        try
        {
            var specs = new GeneralSpecifications<TenantReview>(t => t.UserId == userId && t.TenantId == tenantId);
            var tenantReview = await _unitOfWork.Repository<TenantReview>().GetWithSpecifications(specs);
            if (tenantReview is null)
                return new ResponseHandler().NotFound<string>("Tenant review not found");

            _unitOfWork.Repository<TenantReview>().Delete(tenantReview);
            await _unitOfWork.Save();

            return new ResponseHandler().Success("Tenant review deleted successfully");
        }
        catch (Exception e)
        {
            Log.Error(e, "Error deleting tenant review: {Message}", e.Message);
            return new ResponseHandler().BadRequest<string>("Something went wrong");
        }
    }
    
    public async Task<Response<PaginationToReturnDto<TenantReviewReadDto>>> GetAllTenantReviews(TenantReviewQueryParameters queryParameters)
    {
        try
        {
            var searchTerm = queryParameters.SearchTerm?.Trim().ToLower();
            var specs = new GeneralSpecifications<TenantReview>(t =>
                    (queryParameters.TenantId == 0 || t.TenantId == queryParameters.TenantId) &&
                    (string.IsNullOrWhiteSpace(searchTerm) ||
                     t.Name.ToLower().Trim().Contains(searchTerm) ||
                     (t.Comment ?? string.Empty).ToLower().Trim().Contains(searchTerm)) &&
                    (!queryParameters.Rating.HasValue || t.Rating == queryParameters.Rating.Value),
                queryParameters.PageIndex, queryParameters.PageSize);

            var tenantReviews = await _unitOfWork.Repository<TenantReview>().GetAllWithSpecificationsAndProjection(
                specs,
                t => new TenantReviewReadDto()
                {
                    Id = t.Id,
                    TenantId = t.TenantId,
                    UserId = t.UserId,
                    Name = t.Name,
                    ImageUrl = t.ImageUrl,
                    Rating = t.Rating,
                    Comment = t.Comment
                });

            var count = await _unitOfWork.Repository<TenantReview>().GetCountByCriteriaAsync(t =>
                (queryParameters.TenantId == 0 || t.TenantId == queryParameters.TenantId) &&
                (string.IsNullOrWhiteSpace(searchTerm) ||
                 (t.Name ?? string.Empty).ToLower().Trim().Contains(searchTerm) ||
                 (t.Comment ?? string.Empty).ToLower().Trim().Contains(searchTerm)) &&
                (!queryParameters.Rating.HasValue || t.Rating == queryParameters.Rating.Value));

            var imageTasks = tenantReviews
                .Where(t => !string.IsNullOrWhiteSpace(t.ImageUrl))
                .Select(async t => t.ImageUrl = await DocumentSetting.GetPreSignedUrlAsync(t.ImageUrl!));

            await Task.WhenAll(imageTasks);

            var response = new PaginationToReturnDto<TenantReviewReadDto>(
                count,
                CalcTotalPages(count, queryParameters.PageSize),
                queryParameters.PageSize,
                queryParameters.PageIndex,
                tenantReviews);

            return new ResponseHandler().Success(response);
        }
        catch (Exception e)
        {
            Log.Error(e, "Error getting tenant reviews: {Message}", e.Message);
            return new ResponseHandler().BadRequest<PaginationToReturnDto<TenantReviewReadDto>>("Something went wrong");
        }
    }
    
    public async Task<Response<TenantReviewSummaryDto>> GetTenantReviewSummary(int tenantId)
    {
        try
        {
            
            var query = _unitOfWork.Repository<TenantReview>().AsQueryableAsync(); 

            var grouped = await query
                .Where(r => r.TenantId == tenantId)
                .GroupBy(r => r.Rating)
                .Select(g => new
                {
                    Rating = g.Key,        
                    Count = g.Count()
                })
                .ToListAsync();

            var total = grouped.Sum(x => x.Count);
            var weightedSum = grouped.Sum(x => x.Rating * x.Count);

            var dto = new TenantReviewSummaryDto
            {
                TenantId = tenantId,
                TotalReviews = total,
                AverageRating = total == 0 ? 0 : Math.Round(weightedSum / (double)total, 1),
                Breakdown = new List<RatingBucketDto>()
            };

            for (int stars = 5; stars >= 1; stars--)
            {
                var item = grouped.FirstOrDefault(x => x.Rating == stars);
                var count = item?.Count ?? 0;

                dto.Breakdown.Add(new RatingBucketDto
                {
                    Stars = stars,
                    Count = count,
                    Percentage = total == 0 ? 0 : Math.Round((count * 100.0) / total, 2)
                });
            }

            return new ResponseHandler().Success(dto);
        }
        catch (Exception e)
        {
            Log.Error(e, "Error getting tenant review summary: {Message}", e.Message);
            return new ResponseHandler().BadRequest<TenantReviewSummaryDto>("Something went wrong");
        }
    }

    private static int CalcTotalPages(int count, int pageSize)
    {
        if (count <= 0 || pageSize <= 0) return 0;
        return (int)Math.Ceiling(count / (double)pageSize);
    }
}
