


using MyEntityMembershipService.Repositories.GenericRepo;
using System.Threading.Tasks;

namespace MyEntityMembershipService.Repositories.Entity.BranchOperatingHoursRepo
{
    public interface IBranchOperatingHoursRepository : IGenericRepository<BranchOperatingHours>
    {
        Task<BranchOperatingHours> GetBranchOperatingHoursByBranchIdAsync(int id);
    }
}
