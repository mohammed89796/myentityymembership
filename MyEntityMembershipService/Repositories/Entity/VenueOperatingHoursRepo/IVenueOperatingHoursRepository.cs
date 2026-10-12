


using MyEntityMembershipService.Repositories.GenericRepo;
using System.Threading.Tasks;

namespace MyEntityMembershipService.Repositories.Entity.VenueOperatingHoursRepo
{
    public interface IVenueOperatingHoursRepository : IGenericRepository<VenueOperatingHours>
    {
        Task<VenueOperatingHours> GetVenueOperatingHoursByVenueIdAsync(int id);
    }
}
