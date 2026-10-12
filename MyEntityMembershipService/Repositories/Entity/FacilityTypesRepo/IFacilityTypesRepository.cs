


using MyEntityMembershipService.Repositories.GenericRepo;
using System.Threading.Tasks;

namespace MyEntityMembershipService.Repositories.Entity.FacilityTypesRepo
{
    public interface IFacilityTypesRepository : IGenericRepository<FacilityType>
    {
        Task<List<FacilityType>> GetAllFacilityTypesAsync();
    }
}
