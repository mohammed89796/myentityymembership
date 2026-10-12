using MyEntityMembershipService.DTOs.Membership.PackageRequestAuthorizationDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Membership.PackageRequestAuthorizationService
{
    public interface IPackageRequestAuthorizationService
    {
        Task<Response<PackageRequestAuthorizationDto>> AddPackageRequestAuthorization(PackageRequestAuthorizationDto packageRequestAuthorizationDto);
        Task<Response<List<PackageRequestAuthorizationDto>>> AddPackageRequestAuthorizations(List<PackageRequestAuthorizationDto> packageRequestAuthorizationDtos);
        Task<Response<List<PackageRequestAuthorizationDto>>> GetAllPackageRequestAuthorizations();
        Task<Response<PackageMmbertypeFormsAndConfigrations>> GetPackageRequestAuthorizationById(int id);
        Task<Response<PackageRequestAuthorizationDto>> UpdatePackageRequestAuthorization(PackageRequestAuthorizationDto packageRequestAuthorizationDto);
        Task<Response<bool>> DeletePackageRequestAuthorization(int id);
        Task<Response<List<int>>> GetPackageMemberTypeIdsAsync(int packageId);
        Task<Response<List<PackageRequestAuthorizationDto>>> GetPackageRequestAuthorizationsAsync(int packageId);
    }
}
