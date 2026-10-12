using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;




using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.FacilityOperatingHoursRepo
{
    public class FacilityOperatingHoursRepository : GenericRepository<NewFacilityOperatingHours>, IFacilityOperatingHoursRepository
    {
        private readonly EntityMembershipDbContext _context;

        public FacilityOperatingHoursRepository(EntityMembershipDbContext context)
            : base(context)
        {
            _context = context;
        }


        public async Task<NewFacilityOperatingHours> GetFacilityOperatingHoursByFacilityIdAsync(int id)
        {
            return await _context.NewFacilityOperatingHours.Where(x => x.FacilityId == id).FirstOrDefaultAsync();
        }
    }
}
