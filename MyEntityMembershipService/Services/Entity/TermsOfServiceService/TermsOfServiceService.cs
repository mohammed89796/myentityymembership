using MyEntityMembershipService.Data;
using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Entity.TermsOfServiceDtos;
using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.SpecificationPattern;
using Serilog;

namespace MyEntityMembershipService.Services.Entity.TermsOfServiceService;

public class TermsOfServiceService : ITermsOfServiceService
{
    private readonly IUnitOfWork _unitOfWork;
    public TermsOfServiceService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<Response<string>> CreateTermsOfService(TermsOfServiceCreateDto dto)
    {

        try
        {
            string imageUrl = null;
            if (dto.TermsFile != null)
            {
                imageUrl = await DocumentSetting.AddToContainerAndGetUrl(null, null, dto.TermsFile);
            }
        
            var termsOfService = new TermsOfService
            {
                Title = dto.Title,
                Terms = dto.Terms,
                TermsUrl = imageUrl,
                TenantId = dto.TenantId,
            
            };
        
            await _unitOfWork.Repository<TermsOfService>().Add(termsOfService);
            await _unitOfWork.Save();
        
            return new ResponseHandler().Success("Terms of Service Created Successfully");

        }
        catch (Exception e)
        {
            Log.Error(e, "Error creating terms of service: {Message}", e.Message);
            return new ResponseHandler().BadRequest<string>("Something went wrong");
        }
    }

    public async Task<Response<string>> UpdateTermsOfService(TermsOfServiceUpdateDto dto)
    {
        try
        {
            var specs = new GeneralSpecifications<TermsOfService>(t => t.Id == dto.Id && t.TenantId == dto.TenantId);
            var termsOfService = await _unitOfWork.Repository<TermsOfService>().GetWithSpecifications(specs);
            if(termsOfService is null)
                return new ResponseHandler().NotFound<string>("Terms of Service not found");
            string imageUrl = null;
            if (dto.TermsFile != null)
            {
                imageUrl = await DocumentSetting.AddToContainerAndGetUrl(null, null, dto.TermsFile);
            }

            termsOfService.Terms = dto.Terms;
            termsOfService.TermsUrl = imageUrl;
            termsOfService.Title = dto.Title;
            _unitOfWork.Repository<TermsOfService>().Update(termsOfService);
            await _unitOfWork.Save();
            return new ResponseHandler().Success("Terms of Service Updated Successfully");
        }
        catch (Exception e)
        {
            Log.Error(e, "Error updating terms of service: {Message}", e.Message);
            return new ResponseHandler().BadRequest<string>("Something went wrong");
        }
    }

    public async Task<Response<string>> DeleteTermsOfService(int id, int tenantId)
    {
        try
        {
            var specs = new GeneralSpecifications<TermsOfService>(t => t.Id == id && t.TenantId == tenantId);
            var termsOfService = await _unitOfWork.Repository<TermsOfService>().GetWithSpecifications(specs);
            if (termsOfService is null)
                return new ResponseHandler().NotFound<string>("Terms of Service not found");
            _unitOfWork.Repository<TermsOfService>().Delete(termsOfService);
            await _unitOfWork.Save();
            return new ResponseHandler().Success("Terms of Service Deleted Successfully");
        }
        catch (Exception e)
        {
            Log.Error(e, "Error deleting terms of service: {Message}", e.Message);
            return new ResponseHandler().BadRequest<string>("Something went wrong");
        }
    }

    public async Task<Response<TermsOfServiceReadDto>> GetTermsOfService(int id)
    {
        try
        {
            var specs = new GeneralSpecifications<TermsOfService>(t => t.Id == id);
            var termsOfService = await _unitOfWork.Repository<TermsOfService>().GetWithSpecificationsAndProjection(
                specs, t => new TermsOfServiceReadDto()
                {
                    Id = t.Id,
                    TenantId = t.TenantId,
                    Title = t.Title,
                    Terms = t.Terms,
                    TermsUrl = t.TermsUrl
                });
            
            if (termsOfService is null)
                return new ResponseHandler().NotFound<TermsOfServiceReadDto>("Terms of Service not found");

            termsOfService.TermsUrl = await DocumentSetting.GetPreSignedUrlAsync(termsOfService.TermsUrl!);
            return new ResponseHandler().Success(termsOfService);
        }
        catch (Exception e)
        {
            Log.Error(e, "Error getting terms of service: {Message}", e.Message);
            return new ResponseHandler().BadRequest<TermsOfServiceReadDto>("Something went wrong");
        }

    }

    public async Task<Response<PaginationToReturnDto<TermsOfServiceReadDto>>> GetAllTermsOfService(TermsOfServiceQueryParameters queryParameters)
    {
        try
        {
            var specs = new GeneralSpecifications<TermsOfService>(t =>
                    t.TenantId == queryParameters.TenantId &&
                    (string.IsNullOrWhiteSpace(queryParameters.SearchTerm) || t.Title.ToLower().Trim()
                        .Contains(queryParameters.SearchTerm.Trim().ToLower()))
                , queryParameters.PageIndex, queryParameters.PageSize);
            var termsOfService = await _unitOfWork.Repository<TermsOfService>().GetAllWithSpecificationsAndProjection(
                specs, t => new TermsOfServiceReadDto()
                {
                    Id = t.Id,
                    TenantId = t.TenantId,
                    Title = t.Title,
                    Terms = t.Terms,
                });
            var count = await _unitOfWork.Repository<TermsOfService>().GetCountByCriteriaAsync(t =>
                t.TenantId == queryParameters.TenantId &&
                (string.IsNullOrWhiteSpace(queryParameters.SearchTerm) ||
                 t.Title.ToLower().Trim().Contains(queryParameters.SearchTerm.Trim().ToLower()))
            );

            var task = termsOfService.Select(async t =>
                t.TermsUrl = await DocumentSetting.GetPreSignedUrlAsync(t.TermsUrl!));
            
            await Task.WhenAll(task);
            
            var response = new PaginationToReturnDto<TermsOfServiceReadDto>(count,
                CalcTotalPages(count, queryParameters.PageSize), queryParameters.PageSize, queryParameters.PageIndex,
                termsOfService);
            return new ResponseHandler().Success(response);
        }
        catch (Exception e)
        {
            Log.Error(e, "Error getting terms of service: {Message}", e.Message);
            return new ResponseHandler().BadRequest<PaginationToReturnDto<TermsOfServiceReadDto>>("Something went wrong");
        }
    }
    
    private static int CalcTotalPages(int count, int pageSize)
    {
        if (count <= 0 || pageSize <= 0) return 0;
        return (int)Math.Ceiling(count / (double)pageSize);
    }
}