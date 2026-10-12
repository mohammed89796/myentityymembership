

using MyEntityMembershipService.Repositories.GenericRepo;
using System.Threading.Tasks;

namespace MyEntityMembershipService.Repositories.Entity.BranchLocationRepo
{
    public interface IBranchLocationRepository : IGenericRepository<BranchLocation>
    {
        Task<BranchLocation> GetBranchLocationhByBranchIdAsync(int id);
    }
}
