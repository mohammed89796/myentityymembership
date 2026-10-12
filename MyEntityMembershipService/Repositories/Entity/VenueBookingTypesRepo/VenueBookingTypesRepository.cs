using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;




using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.VenueBookingTypesRepo
{
    public class VenueBookingTypesRepository : GenericRepository<VenueBookingTypes>, IVenueBookingTypesRepository
    {
        private readonly EntityMembershipDbContext _context;

        public VenueBookingTypesRepository(EntityMembershipDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<VenueBookingTypes>> GetAllVenueBookingTypeAsync()
        {
            return await _context.VenueBookingTypes.ToListAsync();
        }
    }
}
