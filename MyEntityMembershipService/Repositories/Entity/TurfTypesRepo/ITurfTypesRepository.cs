


using MyEntityMembershipService.Repositories.GenericRepo;
using System.Threading.Tasks;

namespace MyEntityMembershipService.Repositories.Entity.TurfTypesRepo
{
    public interface ITurfTypesRepository : IGenericRepository<TurfTypes>
    {
        Task<List<TurfTypes>> GetAllTurfTypesAsync();
    }
}
