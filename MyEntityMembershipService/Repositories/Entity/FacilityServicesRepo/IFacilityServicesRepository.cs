

using MyEntityMembershipService.Repositories.GenericRepo;
using System.Threading.Tasks;

namespace MyEntityMembershipService.Repositories.Entity.FacilityServicesRepo
{
    public interface IFacilityServicesRepository : IGenericRepository<FacilityServices>
    {
        Task<List<CoreFacilityServices>> GetAllFacilityServicesAsync();
    }
}
