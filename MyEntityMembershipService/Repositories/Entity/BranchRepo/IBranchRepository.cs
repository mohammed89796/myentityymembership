
using MyEntityMembershipService.DTOs.Entity.BranchDtos;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.BranchRepo
{
    public interface IBranchRepository : IGenericRepository<Branch>
    {
        Task<IEnumerable<Branch>> GetAllBranchsAsync();
        Task<Branch> GetBranchByIdAsync(int id);
        Task<IEnumerable<Branch>> GetBranchByTenantIdAsync(int tenantId, string? searchTerm, int? pageIndex, int? pageSize);
        Task<BranchDetailsDto> GetBranchDetailsByIdAsync(int branchID);
        Task<EditBranchDetailsDto> GetBranchEditDetailsByIdAsync(int branchID);
        Task<BranchListDetailsDto> GetBranchCardsAsync(int pageNumber, int tenantID);
        Task<IEnumerable<int>> GetVenuesOfTenantByIdAsync(int tenantId);
        Task<BranchMobileCardListDto> GetBranchCardsByTenantID(int pageNumber, int tenantId);
        Task<BranchMobileDetailsDto> GetBranchDetailsMobileView(int branchID);
        Task<List<MobileBranchFacilitiesDto>> GetMobileBranchFacilitiesAsync(int branchID);

    }
}
