using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Entity.BranchDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.BranchService
{
    public interface IBranchService
    {
        Task<Response<BranchDto>> GetBranchByIdAsync(int branchId);
        Task<Response<List<BranchDto>>> GetAllBranchesAsync();
        Task<Response<List<BranchDto>>> GetAllBranchesByTenantIdAsync(int? tenantId,string userId);
        Task<Response<AddBranchDto>> AddBranchAsync(AddBranchDto branchDto);
        Task<Response<bool>> AddBranchOperatingHoursAsync(BranchOperatingHoursDto? operatingHoursDto);
        Task<Response<BranchDto>> UpdateBranchAsync(BranchDto branchDto);
        Task<Response<List<BranchDto>>> GetBranchByTenantIdAsync(int tenantId,string? searchTerm, int? pageIndex, int? pageSize);
        Task<Response<BranchDetailsDto>> GetBranchDetailsByIdAsync(int branchID);
        Task<Response<BranchListDetailsDto>> GetBranchCardList(int pageNumber, int tenantID);
        Task<Response<bool>> RemoveBranchAsync(int branchId);
        Task<Response<bool>> RemoveBranchMediaAsync(int branchId, int mediaId);
        Task<Response<EditBranchDto>> EditBranchAsync(EditBranchDto branchDto);
        Task<Response<IEnumerable<int>>> GetVenuesOfTenantByIdAsync(int tenantId);
        Task<Response<EditBranchDetailsDto>> GetBranchEditDetailsByIdAsync(int branchID);
        Task<Response<BranchMobileCardListDto>> GetBranchCardsByTenantID(int pageNumber, int tenantId);
        Task<Response<BranchMobileDetailsDto>> GetBranchMobileDetailsByID(int branchID);
        Task<Response<List<MobileBranchFacilitiesDto>>> GetMobileBranchFacilities(int branchID);
        Task<Response<bool>> AddBranchNewMedia(List<IFormFile> newMedia, int branchID);
        Task<Response<bool>> UpdateBranchOperatingHoursAsync(EditBranchOperatingHoursDto branchDto);
        Task<Response<PaginationToReturnDto<BranchLookupDto>>> GetBranchesByTenantAsync(BranchLookupSearchDto dto);

    }
}
