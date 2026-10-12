


using MyEntityMembershipService.Repositories.GenericRepo;
using System.Threading.Tasks;

namespace MyEntityMembershipService.Repositories.Entity.AmenitiesRepo
{
    public interface IAmenitiesRepository : IGenericRepository<Amenities>
    {
        Task<List<Amenities>> GetAllAmenitiesAsync();
    }
}
