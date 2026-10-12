

using MyEntityMembershipService.Repositories.GenericRepo;
using System.Threading.Tasks;

namespace MyEntityMembershipService.Repositories.Entity.CoreServicesRepo
{
    public interface ICoreServicesRepository : IGenericRepository<CoreService>
    {
        Task<List<CoreService>> GetAllCoreServicesAsync();
    }
}
