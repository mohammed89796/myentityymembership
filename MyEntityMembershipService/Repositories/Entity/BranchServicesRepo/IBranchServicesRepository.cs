


using MyEntityMembershipService.Repositories.GenericRepo;
using System.Threading.Tasks;

namespace MyEntityMembershipService.Repositories.Entity.BranchServicesRepo
{
    public interface IBranchServicesRepository : IGenericRepository<BranchServices>
    {
        Task<List<BranchServices>> GetBranchServicesByBranchIdAsync(int id);
    }
}
