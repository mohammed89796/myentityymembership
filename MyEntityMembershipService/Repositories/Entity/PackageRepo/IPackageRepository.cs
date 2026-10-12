
using MyEntityMembershipService.DTOs.Entity.PackageDtos;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.PackageRepo
{
    public interface IPackageRepository : IGenericRepository<Package>
    {
        Task<IReadOnlyList<Package>> GetAllPackageByTenantId(int? tenantId);
        Task<Package> GetPackageById(int Id);
        Task<IReadOnlyList<Package>> GetPackageByTenantId(int tenantId);
        Task<IReadOnlyList<GetPackageDto>> GetAllPackagesByTenantId(int tenantId);
        Task<IReadOnlyList<GetPackageDto>> GetPackagesByTenantIdAndByListOfIds(int tenantId, List<int> packageIds);
        Task SoftDeletePackage(int id);
        Task<Package> GetPackageWithTenant(int id);
    }
}