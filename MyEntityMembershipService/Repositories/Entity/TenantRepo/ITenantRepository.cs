using MyEntityMembershipService.Data;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.TenantRepo
{
    public interface ITenantRepository : IGenericRepository<Tenant>
    {
        Task<IEnumerable<Tenant>> GetAllTenantsAsync(int typeId);
        Task<Tenant> GetTenantByIdAsync(int id, int typeId);
        Task<IReadOnlyList<Tenant>> GetTenantsByListOfIdsAsync(List<int> tenantIds);
        Task<Tenant> GeFedByIdAsync(int id, int typeId);
        Task<IEnumerable<Tenant>> GetAllFedrationsAsync(int typeId);
        Task<string> CheckEntityType(int Id);

    }
}
