using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Membership.PackageDtos;
using MyEntityMembershipService.DTOs.Membership.PackageDtos.MyEntityMembershipService.DTOs.Membership.PackageDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Membership.PackageServ
{
    public interface IPackageService
    {
        Task<Response<PackageDto>> AddPackage(AddPackageWithSubPackagesDto dto);
        Task<Response<PaginationToReturnDto<GetAllPackageResponseDto>>> GetAllPackagesAsync(GetAllPackageFilterResponseDto filter);
        Task<Response<string>> UpdatePackageActivationStatusAsync(UpdatePackageActivitionDto dto);
        Task<Response<PackageResponseDto>> GetPackageByIdAsync(int packageId,int? tenantId);
        Task<Response<PaginationToReturnDto<PackageMemberDto>>> GetAllPackageMemberAsync(int? tenantId ,int packageId ,int pageSize = 10, int pageNumber = 1);
        Task<Response<PaginationToReturnDto<ActiveRequestResponseDto>>> GetPackageActiveRequestsAsync(
        int packageId, ActiveRequestFilter filter, int pageSize = 10, int pageNumber = 1);
        Task<Response<List<DTOs.Entity.PackageDtos.SubMemberCategoryDto>>> GetAvailableMembersAsync(DTOs.Entity.PackageDtos.GetAvailableMembersRequest request);
        Task<Response<List<SubPackageResponsDto>>> GetAllSubPackagesAsync(int? tenantId ,int packageId);
        Task<Response<PackageBenefitsDto>> GetPackageBenefitsAsync(int? tenantId, int packageId);
    }
}
