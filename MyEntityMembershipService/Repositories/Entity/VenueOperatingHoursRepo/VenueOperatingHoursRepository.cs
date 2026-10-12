using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.VenueOperatingHoursRepo
{
    public class VenueOperatingHoursRepository : GenericRepository<VenueOperatingHours>, IVenueOperatingHoursRepository
    {
        private readonly EntityMembershipDbContext _context;

        public VenueOperatingHoursRepository(EntityMembershipDbContext context)
            : base(context)
        {
            _context = context;
        }


        public async Task<VenueOperatingHours> GetVenueOperatingHoursByVenueIdAsync(int id)
        {
            return await _context.VenueOperatingHours.Where(x => x.NewVenueId == id).FirstOrDefaultAsync();
        }
    }
}
