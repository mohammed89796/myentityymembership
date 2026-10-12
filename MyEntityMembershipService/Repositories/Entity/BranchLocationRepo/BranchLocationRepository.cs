using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;



using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.BranchLocationRepo
{
    public class BranchLocationRepository : GenericRepository<BranchLocation>, IBranchLocationRepository
    {
        private readonly EntityMembershipDbContext _context;

        public BranchLocationRepository(EntityMembershipDbContext context)
            : base(context)
        {
            _context = context;
        }


        public async Task<BranchLocation> GetBranchLocationhByBranchIdAsync(int id)
        {
            return await _context.BranchLocation.Where(x => x.BranchID == id).FirstOrDefaultAsync();
        }
    }
}
