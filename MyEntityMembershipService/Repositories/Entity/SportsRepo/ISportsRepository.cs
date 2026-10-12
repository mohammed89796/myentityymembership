

using MyEntityMembershipService.Repositories.GenericRepo;
using System.Threading.Tasks;

namespace MyEntityMembershipService.Repositories.Entity.SportsRepo
{
    public interface ISportsRepository : IGenericRepository<Sports>
    {
        Task<List<Sports>> GetAllSportsAsync();
    }
}
