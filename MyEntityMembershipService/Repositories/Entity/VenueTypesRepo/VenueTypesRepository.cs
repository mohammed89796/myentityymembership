using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;




using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.VenueTypesRepo
{
    public class VenueTypesRepository : GenericRepository<VenueTypes>, IVenueTypesRepository
    {
        private readonly EntityMembershipDbContext _context;

        public VenueTypesRepository(EntityMembershipDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<VenueTypes>> GetAllVenueTypesAsync()
        {
            return await _context.VenueTypes.ToListAsync();
        }
    }
}
