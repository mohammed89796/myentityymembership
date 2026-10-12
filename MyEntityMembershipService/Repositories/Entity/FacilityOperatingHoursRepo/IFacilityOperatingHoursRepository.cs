


using MyEntityMembershipService.Repositories.GenericRepo;
using System.Threading.Tasks;

namespace MyEntityMembershipService.Repositories.Entity.FacilityOperatingHoursRepo
{
    public interface IFacilityOperatingHoursRepository : IGenericRepository<NewFacilityOperatingHours>
    {
        Task<NewFacilityOperatingHours> GetFacilityOperatingHoursByFacilityIdAsync(int id);
    }
}
