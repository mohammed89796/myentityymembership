using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;




using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.CoreUtilitiesRepo
{
    public class CoreUtilitiesRepository : GenericRepository<NewCoreUtilities>, ICoreUtilitiesRepository
    {
        private readonly EntityMembershipDbContext _context;

        public CoreUtilitiesRepository(EntityMembershipDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<NewCoreUtilities>> GetAllCoreUtilitiesAsync()
        {
            return await _context.NewCoreUtilities.ToListAsync();
        }
    }
}
