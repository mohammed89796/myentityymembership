


using MyEntityMembershipService.Repositories.GenericRepo;
using System.Threading.Tasks;

namespace MyEntityMembershipService.Repositories.Entity.CoreUtilitiesRepo
{
    public interface ICoreUtilitiesRepository : IGenericRepository<NewCoreUtilities>
    {
        Task<List<NewCoreUtilities>> GetAllCoreUtilitiesAsync();
    }
}
