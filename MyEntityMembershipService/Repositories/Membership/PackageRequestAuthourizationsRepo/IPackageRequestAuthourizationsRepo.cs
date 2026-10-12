using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.DTOs.Membership.PackageRequestAuthorizationDtos;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Membership.PackageRequestAuthourizationsRepo
{
    public interface IPackageRequestAuthorizationRepository :IGenericRepository<PackageRequestAuthourizations>
    {

        Task<List<PackageRequestAuthourizations>> GetAuthorizationsByPackageIdAsync(int packageId);
        Task<List<PackageMemberTypeDto>> GetMemberTypesWithRequestTypesAndForms(int Id);
        Task<List<int>> GetPackageMemberTypeIdsAsync(int packageId);
        Task<List<PackageRequestAuthourizations>> GetPackageAuthorizations(int packageId);
        Task<List<PaakageMemberForm>> GetPaakageMemberForms(int packageId);

    }

}
