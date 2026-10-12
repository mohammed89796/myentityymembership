global using MyEntityMembershipService.Data.Entity.Entities;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.VenueTypesRepo
{
    public interface IVenueTypesRepository : IGenericRepository<VenueTypes>
    {
        Task<List<VenueTypes>> GetAllVenueTypesAsync();
    }
}
