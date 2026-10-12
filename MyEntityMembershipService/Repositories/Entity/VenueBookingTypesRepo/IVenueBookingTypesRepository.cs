


using MyEntityMembershipService.Repositories.GenericRepo;
using System.Threading.Tasks;

namespace MyEntityMembershipService.Repositories.Entity.VenueBookingTypesRepo
{
    public interface IVenueBookingTypesRepository : IGenericRepository<VenueBookingTypes>
    {
        Task<List<VenueBookingTypes>> GetAllVenueBookingTypeAsync();
    }
}
