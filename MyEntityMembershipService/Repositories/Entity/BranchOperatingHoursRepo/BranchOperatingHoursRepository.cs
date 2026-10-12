using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;




using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.BranchOperatingHoursRepo
{
    public class BranchOperatingHoursRepository : GenericRepository<BranchOperatingHours>, IBranchOperatingHoursRepository
    {
        private readonly EntityMembershipDbContext _context;

        public BranchOperatingHoursRepository(EntityMembershipDbContext context)
            : base(context)
        {
            _context = context;
        }


        public async Task<BranchOperatingHours> GetBranchOperatingHoursByBranchIdAsync(int id)
        {
            return await _context.BranchOperatingHours.Where(x => x.BranchID == id).FirstOrDefaultAsync();
        }
    }
}
