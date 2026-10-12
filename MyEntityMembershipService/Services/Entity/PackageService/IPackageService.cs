using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Entity.BranchDtos;
using MyEntityMembershipService.DTOs.Entity.PackageDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.PackageService
{
    public interface IPackageService
    {
        Task<Response<PackageDto>> AddPackage(AddPackageDto addPackageDto, IFormFile image);
        Task<Response<List<PackageDto>>> GetAllPackageByTenantId(int? tenantId);
        Task<Response<PackageDto>> GetPackageById(int packageId);
        Task<Response<PackageDataDto>> GetPackageByIdAsync(int packageId);

        Task<Response<List<PackageDto>>> GetPackagesByTenantIdAsync(int tenantId);
        Task<Response<List<PackageDto>>> GetSubscribedPackagesByTenantId(int tenatId);
        Task<Response<bool>> SoftDeletePackage(int packageId);

        Task<Response<IReadOnlyList<GetPackageDto>>> GetAllPackagesByTenantId(int tenantId);

        Task<Response<IReadOnlyList<GetPackageDto>>> GetAllPackagesByTenantIdAndListOfIds(int tenantId, List<int> packagesIds);
        Task<bool> IsPPackageHasAccessToVenueOrFacilityOrBranch(BranchFacilityVenueDto dto);
        Task<List<PackageSummaryDto>> GetSuperPackagesAsync();
        Task<List<PackageSummaryDto>> GetSupPackagesAsync(int superPackageId);
        Task<Response<PaginationToReturnDto<BranchCardViewDto>>> GetPackageBranchesAsync(int PackageId, PackageBranchListDto branchListDetailsDto);
        Task<Response<List<BranchWithFacilitiesDto>>> GetAccessedFacilitiesWithVenuesAsync(int packageId, List<int> branchIds);
    }
}
